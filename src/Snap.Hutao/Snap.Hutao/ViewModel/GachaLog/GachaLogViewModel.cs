// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Page;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.ViewModel.GachaLog;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class GachaLogViewModel : Abstraction.ViewModel
{
    private readonly HutaoCloudStatisticsViewModel hutaoCloudStatisticsViewModel;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly HutaoCloudViewModel hutaoCloudViewModel;
    private readonly ILogger<GachaLogViewModel> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly IProgressFactory progressFactory;
    private readonly IGachaLogService gachaLogService;
    private readonly IInfoBarService infoBarService;
    private readonly ITaskContext taskContext;

    private AdvancedDbCollectionView<GachaArchive>? archives;
    private GachaStatistics? statistics;
    private bool isAggressiveRefresh;
    private bool suppressCurrentItemChangedHandling;

    public AdvancedDbCollectionView<GachaArchive>? Archives
    {
        get => archives;
        set
        {
            if (Archives is not null)
            {
                Archives.CurrentChanged -= OnCurrentArchiveChanged;
            }

            SetProperty(ref archives, value);

            if (value is not null)
            {
                value.CurrentChanged += OnCurrentArchiveChanged;
            }
        }
    }

    public GachaStatistics? Statistics
    {
        get => statistics;
        set
        {
            if (SetProperty(ref statistics, value))
            {
                statistics?.HistoryWishes.MoveCurrentToFirst();
            }
        }
    }

    public bool IsAggressiveRefresh { get => isAggressiveRefresh; set => SetProperty(ref isAggressiveRefresh, value); }

    public HutaoCloudViewModel HutaoCloudViewModel { get => hutaoCloudViewModel; }

    public HutaoCloudStatisticsViewModel HutaoCloudStatisticsViewModel { get => hutaoCloudStatisticsViewModel; }

    protected override async ValueTask<bool> InitializeOverrideAsync()
    {
        try
        {
            if (await gachaLogService.InitializeAsync(CancellationToken).ConfigureAwait(false))
            {
                ArgumentNullException.ThrowIfNull(gachaLogService.Archives);
                using (await EnterCriticalSectionAsync().ConfigureAwait(false))
                {
                    await taskContext.SwitchToMainThreadAsync();
                    Archives = gachaLogService.Archives;
                    HutaoCloudViewModel.RetrieveCommand = RetrieveFromCloudCommand;
                    Archives.MoveCurrentTo(Archives.SourceCollection.SelectedOrFirstOrDefault());
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

        UpdateStatisticsAsync(Archives?.CurrentItem).SafeForget(logger);
    }

    [Command("RefreshByWebCacheCommand")]
    private async Task RefreshByWebCacheAsync()
    {
        await RefreshCoreAsync(RefreshOption.WebCache).ConfigureAwait(false);
    }

    [Command("RefreshBySTokenCommand")]
    private async Task RefreshBySTokenAsync()
    {
        await RefreshCoreAsync(RefreshOption.SToken).ConfigureAwait(false);
    }

    [Command("RefreshByManualInputCommand")]
    private async Task RefreshByManualInputAsync()
    {
        await RefreshCoreAsync(RefreshOption.ManualInput).ConfigureAwait(false);
    }

    private async ValueTask RefreshCoreAsync(RefreshOption option)
    {
        IGachaLogQueryProvider provider = serviceProvider.GetRequiredKeyedService<IGachaLogQueryProvider>(option);
        (bool isOk, GachaLogQuery query) = await provider.GetQueryAsync().ConfigureAwait(false);

        if (!isOk)
        {
            if (!string.IsNullOrEmpty(query.Message))
            {
                infoBarService.Warning(query.Message);
            }

            return;
        }

        RefreshStrategyKind strategy = IsAggressiveRefresh ? RefreshStrategyKind.AggressiveMerge : RefreshStrategyKind.LazyMerge;

        GachaLogRefreshProgressDialog dialog = await contentDialogFactory.CreateInstanceAsync<GachaLogRefreshProgressDialog>().ConfigureAwait(false);

        ContentDialogScope hideToken;
        try
        {
            hideToken = await dialog.BlockAsync(contentDialogFactory).ConfigureAwait(false);
        }
        catch (COMException ex)
        {
            if (ex.HResult == HRESULT.E_ASYNC_OPERATION_NOT_STARTED)
            {
                infoBarService.Error(ex);
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
                        authkeyValid = await gachaLogService.RefreshGachaLogAsync(query, strategy, progress, CancellationToken).ConfigureAwait(false);
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
                    infoBarService.Error(ex);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // We set true here in order to hide the dialog.
            authkeyValid = true;
            infoBarService.Warning(SH.ViewModelGachaLogRefreshOperationCancel);
        }

        await taskContext.SwitchToMainThreadAsync();
        if (authkeyValid)
        {
            await hideToken.DisposeAsync().ConfigureAwait(false);
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
        serviceProvider.GetRequiredService<INavigationService>().Navigate<SettingPage>(INavigationAwaiter.Default, true);
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
            GachaStatistics statistics = await gachaLogService.GetStatisticsAsync(archive).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            Statistics = statistics;
            IsInitialized = true;
        }
        catch (HutaoException ex)
        {
            infoBarService.Error(ex);
        }
    }
}