// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Data;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.ViewModel.Setting;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.ViewModel.GachaLog;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class GachaLogViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IProgressFactory progressFactory;
    private readonly IGachaLogService gachaLogService;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private bool suppressCurrentItemChangedHandling;
    private GachaLogServiceMetadataContext? metadataContext;

    [GeneratedConstructor]
    public partial GachaLogViewModel(IServiceProvider serviceProvider);

    public partial HutaoCloudStatisticsViewModel HutaoCloudStatisticsViewModel { get; }

    public partial WishCountdownViewModel WishCountdownViewModel { get; }

    public partial HutaoCloudViewModel HutaoCloudViewModel { get; }

    public IAdvancedDbCollectionView<GachaArchive>? Archives
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentArchiveChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(field, OnCurrentArchiveChanged);
        }
    }

    public GachaStatistics? Statistics
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                field?.HistoryWishes.MoveCurrentToFirst();
            }
        }
    }

    [ObservableProperty]
    public partial bool IsAggressiveRefresh { get; set; }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        try
        {
            if (!await metadataService.InitializeAsync().ConfigureAwait(false))
            {
                return false;
            }

            metadataContext = await metadataService.GetContextAsync<GachaLogServiceMetadataContext>(token).ConfigureAwait(false);
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                IAdvancedDbCollectionView<GachaArchive> archives = await gachaLogService.GetArchiveCollectionAsync().ConfigureAwait(false);
                await taskContext.SwitchToMainThreadAsync();
                Archives = archives;
                HutaoCloudViewModel.RetrieveCommand = RetrieveFromCloudCommand;
                Archives.MoveCurrentTo(Archives.Source.SelectedOrFirstOrDefault());
            }

            // When `Archives.CurrentItem` is not null, the `Initialization` actually completed in
            // `UpdateStatisticsAsync`, so we return false to make the view hide until the actual
            // initialization is complete. But we return true when no archives are available,
            // so that the empty view can show up.
            if (Archives.CurrentItem is null)
            {
                return true;
            }
        }
        catch (OperationCanceledException)
        {
        }

        return false;
    }

    protected override void UninitializeOverride()
    {
        using (Archives?.SuppressChangeCurrentItem())
        {
            Archives = default;
        }
    }

    private void OnCurrentArchiveChanged(object? sender, object? e)
    {
        if (suppressCurrentItemChangedHandling)
        {
            return;
        }

        UpdateStatisticsAsync(Archives?.CurrentItem).SafeForget();
    }

    [Command("RefreshByWebCacheCommand")]
    private async Task RefreshByWebCacheAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Refresh gachalog", "GachaLogViewModel.Command", [("source", "WebCache")]));

        await PrivateRefreshAsync(RefreshOptionKind.WebCache).ConfigureAwait(false);
    }

    [Command("RefreshBySTokenCommand")]
    private async Task RefreshBySTokenAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Refresh gachalog", "GachaLogViewModel.Command", [("source", "SToken")]));

        await PrivateRefreshAsync(RefreshOptionKind.SToken).ConfigureAwait(false);
    }

    [Command("RefreshByManualInputCommand")]
    private async Task RefreshByManualInputAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Refresh gachalog", "GachaLogViewModel.Command", [("source", "Manual Input")]));

        await PrivateRefreshAsync(RefreshOptionKind.ManualInput).ConfigureAwait(false);
    }

    private async ValueTask PrivateRefreshAsync(RefreshOptionKind optionKind)
    {
        GachaLogQuery query;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IGachaLogQueryProvider provider = scope.ServiceProvider.GetRequiredKeyedService<IGachaLogQueryProvider>(optionKind);
            (bool isOk, query) = await provider.GetQueryAsync().ConfigureAwait(false);

            if (!isOk)
            {
                if (!string.IsNullOrEmpty(query.Message))
                {
                    messenger.Send(InfoBarMessage.Warning(query.Message));
                }

                return;
            }
        }

        RefreshStrategyKind strategy = IsAggressiveRefresh ? RefreshStrategyKind.AggressiveMerge : RefreshStrategyKind.LazyMerge;

        GachaLogRefreshProgressDialog dialog;
        try
        {
            dialog = await contentDialogFactory.CreateInstanceAsync<GachaLogRefreshProgressDialog>(serviceProvider).ConfigureAwait(false);
        }
        catch (ObjectDisposedException)
        {
            // Previous query provider operation toke too long, and the service provider is disposed.
            // For example, the SToken query provider can take a long time to perform a network request.
            return;
        }

        BlockDeferral hideToken;
        try
        {
            hideToken = await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false);
        }
        catch (COMException ex)
        {
            if (ex.HResult is HRESULT.E_ASYNC_OPERATION_NOT_STARTED)
            {
                messenger.Send(InfoBarMessage.Error(ex));
                return;
            }

            throw;
        }

        IProgress<GachaLogFetchStatus> progress = progressFactory.CreateForMainThread<GachaLogFetchStatus>(dialog.OnReport);
        bool authkeyValid;

        try
        {
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                try
                {
                    try
                    {
                        suppressCurrentItemChangedHandling = true;
                        ArgumentNullException.ThrowIfNull(metadataContext);
                        authkeyValid = await gachaLogService.RefreshGachaLogAsync(metadataContext, query, strategy, progress, CancellationToken).ConfigureAwait(false);
                    }
                    finally
                    {
                        suppressCurrentItemChangedHandling = false;
                        await UpdateStatisticsAsync(Archives?.CurrentItem).ConfigureAwait(false);
                    }
                }
                catch (HutaoException ex)
                {
                    authkeyValid = false;
                    messenger.Send(InfoBarMessage.Error(ex));
                }
            }
        }
        catch (OperationCanceledException)
        {
            // We set true here in order to hide the dialog.
            authkeyValid = true;
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelGachaLogRefreshOperationCancel));
        }

        await taskContext.SwitchToMainThreadAsync();
        if (authkeyValid)
        {
            hideToken.Dispose();
        }
        else
        {
            // User needs to manually close the dialog
            dialog.Title = SH.ViewModelGachaLogRefreshFail;
            dialog.PrimaryButtonText = SH.ContentDialogConfirmPrimaryButtonText;
            dialog.DefaultButton = ContentDialogButton.Primary;
        }
    }

    [Command("RemoveArchiveCommand")]
    private async Task RemoveArchiveAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove archive", "GachaLogViewModel.Command"));

        if (Archives?.CurrentItem is null)
        {
            return;
        }

        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(
                SH.FormatViewModelGachaLogRemoveArchiveTitle(Archives.CurrentItem.Uid),
                SH.ViewModelGachaLogRemoveArchiveDescription)
            .ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            await gachaLogService.RemoveArchiveAsync(Archives.CurrentItem).ConfigureAwait(false);
        }
    }

    [Command("RetrieveFromCloudCommand")]
    private async Task RetrieveAsync(string? uid)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Retrive records from hutao cloud", "GachaLogViewModel.Command"));

        if (uid is null)
        {
            return;
        }

        try
        {
            suppressCurrentItemChangedHandling = true;
            ValueResult<bool, Guid> result = await HutaoCloudViewModel.RetrieveAsync(uid).ConfigureAwait(false);

            if (result.TryGetValue(out Guid archiveId))
            {
                GachaArchive archive = await gachaLogService.EnsureArchiveInCollectionAsync(archiveId).ConfigureAwait(false);

                await taskContext.SwitchToMainThreadAsync();
                Archives?.MoveCurrentTo(archive);
            }
        }
        finally
        {
            suppressCurrentItemChangedHandling = false;
            await UpdateStatisticsAsync(Archives?.CurrentItem).ConfigureAwait(false);
        }
    }

    [Command("ImportExportCommand")]
    private void ImportExport()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Navigate (Import/Export)", "GachaLogViewModel.Command"));

        INavigationCompletionSource navigationAwaiter = new NavigationExtraData(SettingViewModel.UIGFImportExport);
        serviceProvider.GetRequiredService<INavigationService>().Navigate<SettingPage>(navigationAwaiter, true);
    }

    private async ValueTask UpdateStatisticsAsync(GachaArchive? archive)
    {
        if (archive is null)
        {
            Statistics = default;
            return;
        }

        try
        {
            ArgumentNullException.ThrowIfNull(metadataContext);
            GachaStatistics statistics = await gachaLogService.GetStatisticsAsync(metadataContext, archive).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            Statistics = statistics;
            IsInitialized = true;
        }
        catch (HutaoException ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
        }
    }
}