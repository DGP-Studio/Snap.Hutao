// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Factory.Picker;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.ViewModel.GachaLog;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class GachaLogViewModel : Abstraction.ViewModel
{
    private readonly HutaoCloudStatisticsViewModel hutaoCloudStatisticsViewModel;
    private readonly IGachaLogQueryProviderFactory gachaLogQueryProviderFactory;
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly HutaoCloudViewModel hutaoCloudViewModel;
    private readonly ILogger<GachaLogViewModel> logger;
    private readonly IProgressFactory progressFactory;
    private readonly IGachaLogService gachaLogService;
    private readonly IInfoBarService infoBarService;
    private readonly JsonSerializerOptions options;
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
                    Archives.MoveCurrentTo(Archives.SourceCollection.SelectedOrDefault());
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
        IGachaLogQueryProvider provider = gachaLogQueryProviderFactory.Create(option);
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
            hideToken = await dialog.BlockAsync(taskContext).ConfigureAwait(false);
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

    [Command("ImportFromUIGFJsonCommand")]
    private async Task ImportFromUIGFJsonAsync()
    {
        (bool isOk, ValueFile file) = fileSystemPickerInteraction.PickFile(
            SH.ViewModelGachaUIGFImportPickerTitile,
            [(SH.ViewModelGachaLogExportFileType, "*.json")]);

        if (!isOk)
        {
            return;
        }

        ValueResult<bool, UIGF?> result = await file.DeserializeFromJsonAsync<UIGF>(options).ConfigureAwait(false);
        if (result.TryGetValue(out UIGF? uigf))
        {
            await TryImportUIGFInternalAsync(uigf).ConfigureAwait(false);
        }
        else
        {
            infoBarService.Error(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage);
        }
    }

    [Command("ExportToUIGFJsonCommand")]
    private async Task ExportToUIGFJsonAsync()
    {
        if (Archives?.CurrentItem is null)
        {
            return;
        }

        (bool isOk, ValueFile file) = fileSystemPickerInteraction.SaveFile(
            SH.ViewModelGachaLogUIGFExportPickerTitle,
            $"{Archives.CurrentItem.Uid}.json",
            [(SH.ViewModelGachaLogExportFileType, "*.json")]);

        if (!isOk)
        {
            return;
        }

        UIGF uigf = await gachaLogService.ExportToUIGFAsync(Archives.CurrentItem).ConfigureAwait(false);
        if (await file.SerializeToJsonAsync(uigf, options).ConfigureAwait(false))
        {
            infoBarService.Success(SH.ViewModelExportSuccessTitle, SH.ViewModelExportSuccessMessage);
        }
        else
        {
            infoBarService.Warning(SH.ViewModelExportWarningTitle, SH.ViewModelExportWarningMessage);
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

        ValueResult<bool, Guid> result = await HutaoCloudViewModel.RetrieveAsync(uid).ConfigureAwait(false);

        if (result.TryGetValue(out Guid archiveId))
        {
            GachaArchive archive = await gachaLogService.EnsureArchiveInCollectionAsync(archiveId).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            Archives?.MoveCurrentTo(archive);
        }
    }

    private async ValueTask UpdateStatisticsAsync(GachaArchive? archive)
    {
        if (archive is null)
        {
            IsInitialized = false;
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

    private async ValueTask<bool> TryImportUIGFInternalAsync(UIGF uigf)
    {
        if (!uigf.IsCurrentVersionSupported(out _))
        {
            infoBarService.Warning(SH.ViewModelGachaLogImportWarningTitle, SH.ViewModelGachaLogImportWarningMessage);
            return false;
        }

        GachaLogImportDialog importDialog = await contentDialogFactory.CreateInstanceAsync<GachaLogImportDialog>(uigf).ConfigureAwait(false);
        if (!await importDialog.GetShouldImportAsync().ConfigureAwait(false))
        {
            return false;
        }

        await taskContext.SwitchToMainThreadAsync();
        ContentDialog dialog = await contentDialogFactory.CreateForIndeterminateProgressAsync(SH.ViewModelGachaLogImportProgress).ConfigureAwait(true);
        try
        {
            using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
            {
                try
                {
                    suppressCurrentItemChangedHandling = true;
                    await gachaLogService.ImportFromUIGFAsync(uigf).ConfigureAwait(false);
                }
                finally
                {
                    suppressCurrentItemChangedHandling = false;
                    await UpdateStatisticsAsync(Archives?.CurrentItem).ConfigureAwait(false);
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            // 语言不匹配/导入物品中存在无效的项
            infoBarService.Error(ex);
            return false;
        }
        catch (FormatException ex)
        {
            infoBarService.Error(ex);
            return false;
        }

        infoBarService.Success(SH.ViewModelGachaLogImportComplete);
        return true;
    }
}