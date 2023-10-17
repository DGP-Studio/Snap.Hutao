// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Windows.Storage.Pickers;

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 祈愿记录视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class GachaLogViewModel : Abstraction.ViewModel
{
    private readonly IGachaLogQueryProviderFactory gachaLogQueryProviderFactory;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly HutaoCloudStatisticsViewModel hutaoCloudStatisticsViewModel;
    private readonly HutaoCloudViewModel hutaoCloudViewModel;
    private readonly IGachaLogService gachaLogService;
    private readonly IInfoBarService infoBarService;
    private readonly JsonSerializerOptions options;
    private readonly IPickerFactory pickerFactory;
    private readonly ITaskContext taskContext;

    private ObservableCollection<GachaArchive>? archives;
    private GachaArchive? selectedArchive;
    private GachaStatistics? statistics;
    private bool isAggressiveRefresh;
    private HistoryWish? selectedHistoryWish;

    /// <summary>
    /// 存档集合
    /// </summary>
    public ObservableCollection<GachaArchive>? Archives { get => archives; set => SetProperty(ref archives, value); }

    /// <summary>
    /// 选中的存档
    /// 切换存档时异步获取对应的统计
    /// </summary>
    public GachaArchive? SelectedArchive
    {
        get => selectedArchive;
        set => SetSelectedArchiveAndUpdateStatisticsAsync(value).SafeForget();
    }

    /// <summary>
    /// 当前统计信息
    /// </summary>
    public GachaStatistics? Statistics
    {
        get => statistics;
        set
        {
            if (SetProperty(ref statistics, value))
            {
                SelectedHistoryWish = statistics?.HistoryWishes.FirstOrDefault();
            }
        }
    }

    /// <summary>
    /// 选中的历史祈愿
    /// </summary>
    public HistoryWish? SelectedHistoryWish { get => selectedHistoryWish; set => SetProperty(ref selectedHistoryWish, value); }

    /// <summary>
    /// 是否为贪婪刷新
    /// </summary>
    public bool IsAggressiveRefresh { get => isAggressiveRefresh; set => SetProperty(ref isAggressiveRefresh, value); }

    /// <summary>
    /// 胡桃云服务视图
    /// </summary>
    public HutaoCloudViewModel HutaoCloudViewModel { get => hutaoCloudViewModel; }

    /// <summary>
    /// 胡桃云祈愿统计试图
    /// </summary>
    public HutaoCloudStatisticsViewModel HutaoCloudStatisticsViewModel { get => hutaoCloudStatisticsViewModel; }

    protected override async ValueTask<bool> InitializeUIAsync()
    {
        try
        {
            if (await gachaLogService.InitializeAsync(CancellationToken).ConfigureAwait(false))
            {
                ArgumentNullException.ThrowIfNull(gachaLogService.ArchiveCollection);
                ObservableCollection<GachaArchive> archives = gachaLogService.ArchiveCollection;

                await taskContext.SwitchToMainThreadAsync();
                Archives = archives;
                HutaoCloudViewModel.RetrieveCommand = RetrieveFromCloudCommand;
                await SetSelectedArchiveAndUpdateStatisticsAsync(Archives.SelectedOrDefault(), true).ConfigureAwait(false);
                return true;
            }
        }
        catch (OperationCanceledException)
        {
        }

        return false;
    }

    [Command("RefreshByWebCacheCommand")]
    private Task RefreshByWebCacheAsync()
    {
        return RefreshInternalAsync(RefreshOption.WebCache).AsTask();
    }

    [Command("RefreshBySTokenCommand")]
    private Task RefreshBySTokenAsync()
    {
        return RefreshInternalAsync(RefreshOption.SToken).AsTask();
    }

    [Command("RefreshByManualInputCommand")]
    private Task RefreshByManualInputAsync()
    {
        return RefreshInternalAsync(RefreshOption.ManualInput).AsTask();
    }

    private async ValueTask RefreshInternalAsync(RefreshOption option)
    {
        IGachaLogQueryProvider provider = gachaLogQueryProviderFactory.Create(option);

        (bool isOk, GachaLogQuery query) = await provider.GetQueryAsync().ConfigureAwait(false);

        if (isOk)
        {
            RefreshStrategy strategy = IsAggressiveRefresh ? RefreshStrategy.AggressiveMerge : RefreshStrategy.LazyMerge;

            GachaLogRefreshProgressDialog dialog = await contentDialogFactory.CreateInstanceAsync<GachaLogRefreshProgressDialog>().ConfigureAwait(false);

            ContentDialogHideToken hideToken;
            try
            {
                hideToken = await dialog.BlockAsync(taskContext).ConfigureAwait(false);
            }
            catch (COMException ex)
            {
                if (ex.HResult == unchecked((int)0x80000019))
                {
                    infoBarService.Error(ex);
                    return;
                }

                throw;
            }

            IProgress<GachaLogFetchStatus> progress = taskContext.CreateProgressForMainThread<GachaLogFetchStatus>(dialog.OnReport);
            bool authkeyValid;

            try
            {
                using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        authkeyValid = await gachaLogService.RefreshGachaLogAsync(query, strategy, progress, CancellationToken).ConfigureAwait(false);
                    }
                    catch (UserdataCorruptedException ex)
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
                await SetSelectedArchiveAndUpdateStatisticsAsync(gachaLogService.CurrentArchive, true).ConfigureAwait(false);
                await hideToken.DisposeAsync().ConfigureAwait(false);
            }
            else
            {
                dialog.Title = SH.ViewModelGachaLogRefreshFail;
                dialog.PrimaryButtonText = SH.ContentDialogConfirmPrimaryButtonText;
                dialog.DefaultButton = ContentDialogButton.Primary;
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(query.Message))
            {
                infoBarService.Warning(query.Message);
            }
        }
    }

    [Command("ImportFromUIGFJsonCommand")]
    private async Task ImportFromUIGFJsonAsync()
    {
        FileOpenPicker picker = pickerFactory.GetFileOpenPicker(PickerLocationId.Desktop, SH.FilePickerImportCommit, ".json");
        (bool isPickerOk, ValueFile file) = await picker.TryPickSingleFileAsync().ConfigureAwait(false);
        if (isPickerOk)
        {
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
    }

    [Command("ExportToUIGFJsonCommand")]
    private async Task ExportToUIGFJsonAsync()
    {
        if (SelectedArchive is not null)
        {
            Dictionary<string, IList<string>> fileTypes = new()
            {
                [SH.ViewModelGachaLogExportFileType] = ".json".Enumerate().ToList(),
            };

            FileSavePicker picker = pickerFactory.GetFileSavePicker(
                PickerLocationId.Desktop,
                $"{SelectedArchive.Uid}.json",
                SH.FilePickerExportCommit,
                fileTypes);

            (bool isPickerOk, ValueFile file) = await picker.TryPickSaveFileAsync().ConfigureAwait(false);

            if (isPickerOk)
            {
                UIGF uigf = await gachaLogService.ExportToUIGFAsync(SelectedArchive).ConfigureAwait(false);
                if (await file.SerializeToJsonAsync(uigf, options).ConfigureAwait(false))
                {
                    infoBarService.Success(SH.ViewModelExportSuccessTitle, SH.ViewModelExportSuccessMessage);
                }
                else
                {
                    infoBarService.Warning(SH.ViewModelExportWarningTitle, SH.ViewModelExportWarningMessage);
                }
            }
        }
    }

    [Command("RemoveArchiveCommand")]
    private async Task RemoveArchiveAsync()
    {
        if (Archives is not null && SelectedArchive is not null)
        {
            ContentDialogResult result = await contentDialogFactory
                .CreateForConfirmCancelAsync(SH.ViewModelGachaLogRemoveArchiveTitle.Format(SelectedArchive.Uid), SH.ViewModelGachaLogRemoveArchiveDescription)
                .ConfigureAwait(false);

            if (result == ContentDialogResult.Primary)
            {
                using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
                {
                    await gachaLogService.RemoveArchiveAsync(SelectedArchive).ConfigureAwait(false);

                    // reselect first archive
                    await taskContext.SwitchToMainThreadAsync();
                    await SetSelectedArchiveAndUpdateStatisticsAsync(Archives.FirstOrDefault(), false).ConfigureAwait(false);
                }
            }
        }
    }

    [Command("RetrieveFromCloudCommand")]
    private async Task RetrieveAsync(string? uid)
    {
        if (uid is not null)
        {
            ValueResult<bool, Guid> result = await HutaoCloudViewModel.RetrieveAsync(uid).ConfigureAwait(false);

            if (result.TryGetValue(out Guid archiveId))
            {
                GachaArchive archive = await gachaLogService.EnsureArchiveInCollectionAsync(archiveId).ConfigureAwait(false);

                await taskContext.SwitchToMainThreadAsync();
                await SetSelectedArchiveAndUpdateStatisticsAsync(archive, true).ConfigureAwait(false);
            }
        }
    }

    private async ValueTask SetSelectedArchiveAndUpdateStatisticsAsync(GachaArchive? archive, bool forceUpdate = false)
    {
        if (IsViewDisposed)
        {
            return;
        }

        bool changed = SetProperty(ref selectedArchive, archive, nameof(SelectedArchive));

        if (changed)
        {
            gachaLogService.CurrentArchive = archive;
        }

        if (forceUpdate || changed)
        {
            if (archive is not null)
            {
                await UpdateStatisticsAsync(archive).ConfigureAwait(false);
            }
            else
            {
                // 删光了存档或使用 Ctrl 取消了存档选择时触发
                // 因此我们在这里额外判断一次是否删光了存档
                if (Archives.IsNullOrEmpty())
                {
                    Statistics = null;
                }
            }
        }
    }

    private async ValueTask UpdateStatisticsAsync(GachaArchive? archive)
    {
        try
        {
            GachaStatistics? statistics = await gachaLogService.GetStatisticsAsync(archive).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            Statistics = statistics;
            IsInitialized = true;
        }
        catch (UserdataCorruptedException ex)
        {
            infoBarService.Error(ex);
        }
    }

    private async ValueTask<bool> TryImportUIGFInternalAsync(UIGF uigf)
    {
        if (!uigf.IsCurrentVersionSupported(out UIGFVersion version))
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
                await gachaLogService.ImportFromUIGFAsync(uigf).ConfigureAwait(false);
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
        await taskContext.SwitchToMainThreadAsync();
        await SetSelectedArchiveAndUpdateStatisticsAsync(gachaLogService.CurrentArchive, true).ConfigureAwait(false);
        return true;
    }
}