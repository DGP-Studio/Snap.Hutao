// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;
using Windows.Storage.Pickers;

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 祈愿记录视图模型
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped)]
internal sealed class GachaLogViewModel : Abstraction.ViewModel
{
    private readonly IGachaLogService gachaLogService;
    private readonly IInfoBarService infoBarService;
    private readonly IPickerFactory pickerFactory;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly JsonSerializerOptions options;

    private ObservableCollection<GachaArchive>? archives;
    private GachaArchive? selectedArchive;
    private GachaStatistics? statistics;
    private bool isAggressiveRefresh;
    private HistoryWish? selectedHistoryWish;

    /// <summary>
    /// 构造一个新的祈愿记录视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public GachaLogViewModel(IServiceProvider serviceProvider)
    {
        gachaLogService = serviceProvider.GetRequiredService<IGachaLogService>();
        infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        pickerFactory = serviceProvider.GetRequiredService<IPickerFactory>();
        contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();
        options = serviceProvider.GetRequiredService<JsonSerializerOptions>();

        HutaoCloudViewModel = serviceProvider.GetRequiredService<HutaoCloudViewModel>();

        RefreshByWebCacheCommand = new AsyncRelayCommand(RefreshByWebCacheAsync);
        RefreshBySTokenCommand = new AsyncRelayCommand(RefreshBySTokenAsync);
        RefreshByManualInputCommand = new AsyncRelayCommand(RefreshByManualInputAsync);
        ImportFromUIGFJsonCommand = new AsyncRelayCommand(ImportFromUIGFJsonAsync);
        ExportToUIGFJsonCommand = new AsyncRelayCommand(ExportToUIGFJsonAsync);
        RemoveArchiveCommand = new AsyncRelayCommand(RemoveArchiveAsync);
        RetrieveFromCloudCommand = new AsyncRelayCommand<string>(RetrieveAsync);
    }

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
    public HutaoCloudViewModel HutaoCloudViewModel { get; }

    /// <summary>
    /// 浏览器缓存刷新命令
    /// </summary>
    public ICommand RefreshByWebCacheCommand { get; }

    /// <summary>
    /// SToken 刷新命令
    /// </summary>
    public ICommand RefreshBySTokenCommand { get; }

    /// <summary>
    /// 手动输入Url刷新命令
    /// </summary>
    public ICommand RefreshByManualInputCommand { get; }

    /// <summary>
    /// 从 UIGF Json 导入命令
    /// </summary>
    public ICommand ImportFromUIGFJsonCommand { get; }

    /// <summary>
    /// 导出到 UIGF Json 命令
    /// </summary>
    public ICommand ExportToUIGFJsonCommand { get; }

    /// <summary>
    /// 删除存档命令
    /// </summary>
    public ICommand RemoveArchiveCommand { get; }

    /// <summary>
    /// 从云端获取记录命令
    /// </summary>
    public ICommand RetrieveFromCloudCommand { get; }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        try
        {
            if (await gachaLogService.InitializeAsync(CancellationToken).ConfigureAwait(true))
            {
                ObservableCollection<GachaArchive> archives;
                using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
                {
                    archives = await gachaLogService.GetArchiveCollectionAsync().ConfigureAwait(false);
                }

                await ThreadHelper.SwitchToMainThreadAsync();
                Archives = archives;
                SetSelectedArchiveAndUpdateStatistics(Archives.SelectedOrDefault(), true);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private Task RefreshByWebCacheAsync()
    {
        return RefreshInternalAsync(RefreshOption.WebCache);
    }

    private Task RefreshBySTokenAsync()
    {
        return RefreshInternalAsync(RefreshOption.SToken);
    }

    private Task RefreshByManualInputAsync()
    {
        return RefreshInternalAsync(RefreshOption.ManualInput);
    }

    private async Task RefreshInternalAsync(RefreshOption option)
    {
        IGachaLogQueryProvider? provider = gachaLogService.GetGachaLogQueryProvider(option);

        if (provider != null)
        {
            (bool isOk, GachaLogQuery query) = await provider.GetQueryAsync().ConfigureAwait(false);

            if (isOk)
            {
                RefreshStrategy strategy = IsAggressiveRefresh ? RefreshStrategy.AggressiveMerge : RefreshStrategy.LazyMerge;

                // ContentDialog must be created by main thread.
                await ThreadHelper.SwitchToMainThreadAsync();
                GachaLogRefreshProgressDialog dialog = new();
                IDisposable dialogHider = await dialog.BlockAsync().ConfigureAwait(false);
                Progress<FetchState> progress = new(dialog.OnReport);
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

                await ThreadHelper.SwitchToMainThreadAsync();
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

    private async Task ImportFromUIGFJsonAsync()
    {
        FileOpenPicker picker = pickerFactory.GetFileOpenPicker(PickerLocationId.Desktop, SH.FilePickerImportCommit, ".json");
        (bool isPickerOk, FilePath file) = await picker.TryPickSingleFileAsync().ConfigureAwait(false);
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

    private async Task ExportToUIGFJsonAsync()
    {
        if (SelectedArchive != null)
        {
            FileSavePicker picker = pickerFactory.GetFileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            picker.SuggestedFileName = SelectedArchive.Uid;
            picker.CommitButtonText = SH.FilePickerExportCommit;
            picker.FileTypeChoices.Add(SH.ViewModelGachaLogExportFileType, ".json".Enumerate().ToList());

            (bool isPickerOk, FilePath file) = await picker.TryPickSaveFileAsync().ConfigureAwait(false);

            if (isPickerOk)
            {
                UIGF uigf = await gachaLogService.ExportToUIGFAsync(SelectedArchive).ConfigureAwait(false);
                bool isOk = await file.SerializeToJsonAsync(uigf, options).ConfigureAwait(false);

                if (isOk)
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
                    await ThreadHelper.SwitchToMainThreadAsync();
                    SelectedArchive = Archives.FirstOrDefault();
                }
            }
        }
    }

    private async Task RetrieveAsync(string? uid)
    {
        if (uid != null)
        {
            (bool isOk, GachaArchive? archive) = await HutaoCloudViewModel.RetrieveAsync(uid).ConfigureAwait(false);

            if (isOk)
            {
                await ThreadHelper.SwitchToMainThreadAsync();
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
    /// <param name="forceUpdate">强制刷新，即使Uid相同也刷新该Uid的记录</param>
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

            await ThreadHelper.SwitchToMainThreadAsync();
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
        if (uigf.IsCurrentVersionSupported())
        {
            // ContentDialog must be created by main thread.
            await ThreadHelper.SwitchToMainThreadAsync();
            if (await new GachaLogImportDialog(uigf).GetShouldImportAsync().ConfigureAwait(true))
            {
                if (uigf.IsValidList())
                {
                    ContentDialog dialog = await contentDialogFactory.CreateForIndeterminateProgressAsync(SH.ViewModelGachaLogImportProgress).ConfigureAwait(true);
                    using (await dialog.BlockAsync().ConfigureAwait(false))
                    {
                        await gachaLogService.ImportFromUIGFAsync(uigf.List, uigf.Info.Uid).ConfigureAwait(false);
                    }

                    infoBarService.Success(SH.ViewModelGachaLogImportComplete);
                    await ThreadHelper.SwitchToMainThreadAsync();
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