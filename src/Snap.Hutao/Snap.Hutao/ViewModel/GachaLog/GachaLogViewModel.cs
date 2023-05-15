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
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly HutaoCloudViewModel hutaoCloudViewModel;
    private readonly IServiceProvider serviceProvider;
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
        set => SetSelectedArchiveAndUpdateStatistics(value);
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

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        try
        {
            if (await gachaLogService.InitializeAsync(CancellationToken).ConfigureAwait(false))
            {
                ObservableCollection<GachaArchive> archives = gachaLogService.ArchiveCollection;
                await taskContext.SwitchToMainThreadAsync();
                Archives = archives;
                SetSelectedArchiveAndUpdateStatistics(Archives.SelectedOrDefault(), true);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private static bool CanImport(UIGFVersion version, UIGF uigf)
    {
        if (version == UIGFVersion.Major2Minor3OrHigher)
        {
            return true;
        }
        else if (version == UIGFVersion.Major2Minor2OrLower && uigf.IsMajor2Minor2OrLowerListValid())
        {
            return true;
        }

        return false;
    }

    [Command("RefreshByWebCacheCommand")]
    private Task RefreshByWebCacheAsync()
    {
        return RefreshInternalAsync(RefreshOption.WebCache);
    }

    [Command("RefreshBySTokenCommand")]
    private Task RefreshBySTokenAsync()
    {
        return RefreshInternalAsync(RefreshOption.SToken);
    }

    [Command("RefreshByManualInputCommand")]
    private Task RefreshByManualInputAsync()
    {
        return RefreshInternalAsync(RefreshOption.ManualInput);
    }

    private async Task RefreshInternalAsync(RefreshOption option)
    {
        IGachaLogQueryProvider? provider = serviceProvider.PickProvider(option);

        if (provider != null)
        {
            (bool isOk, GachaLogQuery query) = await provider.GetQueryAsync().ConfigureAwait(false);

            if (isOk)
            {
                RefreshStrategy strategy = IsAggressiveRefresh ? RefreshStrategy.AggressiveMerge : RefreshStrategy.LazyMerge;

                // ContentDialog must be created by main thread.
                await taskContext.SwitchToMainThreadAsync();

                GachaLogRefreshProgressDialog dialog = serviceProvider.CreateInstance<GachaLogRefreshProgressDialog>();
                IDisposable dialogHider = await dialog.BlockAsync(taskContext).ConfigureAwait(false);
                Progress<GachaLogFetchStatus> progress = new(dialog.OnReport);
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
                    SetSelectedArchiveAndUpdateStatistics(gachaLogService.CurrentArchive, true);
                    dialogHider.Dispose();
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
    }

    [Command("ImportFromUIGFJsonCommand")]
    private async Task ImportFromUIGFJsonAsync()
    {
        FileOpenPicker picker = pickerFactory.GetFileOpenPicker(PickerLocationId.Desktop, SH.FilePickerImportCommit, ".json");
        (bool isPickerOk, ValueFile file) = await picker.TryPickSingleFileAsync().ConfigureAwait(false);
        if (isPickerOk)
        {
            (bool isOk, UIGF? uigf) = await file.DeserializeFromJsonAsync<UIGF>(options).ConfigureAwait(false);
            if (isOk)
            {
                await TryImportUIGFInternalAsync(uigf!).ConfigureAwait(false);
            }
            else
            {
                await contentDialogFactory.ConfirmAsync(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage).ConfigureAwait(false);
            }
        }
    }

    [Command("ExportToUIGFJsonCommand")]
    private async Task ExportToUIGFJsonAsync()
    {
        if (SelectedArchive != null)
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
        if (Archives != null && SelectedArchive != null)
        {
            ContentDialogResult result = await contentDialogFactory
                .ConfirmCancelAsync(string.Format(SH.ViewModelGachaLogRemoveArchiveTitle, SelectedArchive.Uid), SH.ViewModelGachaLogRemoveArchiveDescription)
                .ConfigureAwait(false);

            if (result == ContentDialogResult.Primary)
            {
                using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
                {
                    await gachaLogService.RemoveArchiveAsync(SelectedArchive).ConfigureAwait(false);

                    // reselect first archive
                    await taskContext.SwitchToMainThreadAsync();
                    SelectedArchive = Archives.FirstOrDefault();
                }
            }
        }
    }

    [Command("RetrieveFromCloudCommand")]
    private async Task RetrieveAsync(string? uid)
    {
        if (uid != null)
        {
            (bool isOk, GachaArchive? archive) = await HutaoCloudViewModel.RetrieveAsync(uid).ConfigureAwait(false);

            if (isOk)
            {
                await taskContext.SwitchToMainThreadAsync();
                Archives?.AddIfNotContains(archive!);
                SetSelectedArchiveAndUpdateStatistics(archive, true);
            }
        }
    }

    /// <summary>
    /// 设置当前的祈愿存档
    /// 需要从主线程调用
    /// </summary>
    /// <param name="archive">存档</param>
    /// <param name="forceUpdate">强制刷新，即使Uid相同也刷新该 Uid 的记录</param>
    private void SetSelectedArchiveAndUpdateStatistics(GachaArchive? archive, bool forceUpdate = false)
    {
        bool changed = false;
        if (selectedArchive != archive)
        {
            selectedArchive = archive;
            changed = true;
        }

        if (changed)
        {
            gachaLogService.CurrentArchive = archive;
            OnPropertyChanged(nameof(SelectedArchive));
        }

        if (forceUpdate || changed)
        {
            if (archive == null)
            {
                // no gacha log
                IsInitialized = true;
            }
            else
            {
                UpdateStatisticsAsync(archive).SafeForget();
            }
        }
    }

    private async Task UpdateStatisticsAsync(GachaArchive? archive)
    {
        try
        {
            GachaStatistics? temp = await gachaLogService.GetStatisticsAsync(archive).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            Statistics = temp;
            IsInitialized = true;
        }
        catch (UserdataCorruptedException ex)
        {
            infoBarService.Error(ex);
        }
    }

    private async Task<bool> TryImportUIGFInternalAsync(UIGF uigf)
    {
        if (uigf.IsCurrentVersionSupported(out UIGFVersion version))
        {
            // ContentDialog must be created by main thread.
            await taskContext.SwitchToMainThreadAsync();
            GachaLogImportDialog importDialog = serviceProvider.CreateInstance<GachaLogImportDialog>(uigf);
            if (await importDialog.GetShouldImportAsync().ConfigureAwait(true))
            {
                if (CanImport(version, uigf))
                {
                    ContentDialog dialog = await contentDialogFactory.CreateForIndeterminateProgressAsync(SH.ViewModelGachaLogImportProgress).ConfigureAwait(true);
                    using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
                    {
                        await gachaLogService.ImportFromUIGFAsync(uigf).ConfigureAwait(false);
                    }

                    infoBarService.Success(SH.ViewModelGachaLogImportComplete);
                    await taskContext.SwitchToMainThreadAsync();
                    SetSelectedArchiveAndUpdateStatistics(gachaLogService.CurrentArchive, true);
                    return true;
                }
                else
                {
                    infoBarService.Warning(SH.ViewModelGachaLogImportWarningTitle, SH.ViewModelGachaLogImportWarningMessage2);
                }
            }
        }
        else
        {
            infoBarService.Warning(SH.ViewModelGachaLogImportWarningTitle, SH.ViewModelGachaLogImportWarningMessage);
        }

        return false;
    }
}