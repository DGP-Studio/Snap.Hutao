// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Extension;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Binding.Gacha;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;
using Windows.Storage.Pickers;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 祈愿记录视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class GachaLogViewModel : Abstraction.ViewModel
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
    /// <param name="gachaLogService">祈愿记录服务</param>
    /// <param name="infoBarService">信息</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="contentDialogFactory">内容对话框工厂</param>
    /// <param name="pickerFactory">文件选择器工厂</param>
    public GachaLogViewModel(
        IGachaLogService gachaLogService,
        IInfoBarService infoBarService,
        JsonSerializerOptions options,
        IContentDialogFactory contentDialogFactory,
        IPickerFactory pickerFactory)
    {
        this.gachaLogService = gachaLogService;
        this.infoBarService = infoBarService;
        this.pickerFactory = pickerFactory;
        this.contentDialogFactory = contentDialogFactory;
        this.options = options;

        OpenUICommand = new AsyncRelayCommand(OpenUIAsync);
        RefreshByWebCacheCommand = new AsyncRelayCommand(RefreshByWebCacheAsync);
        RefreshByStokenCommand = new AsyncRelayCommand(RefreshByStokenAsync);
        RefreshByManualInputCommand = new AsyncRelayCommand(RefreshByManualInputAsync);
        ImportFromUIGFJsonCommand = new AsyncRelayCommand(ImportFromUIGFJsonAsync);
        ExportToUIGFJsonCommand = new AsyncRelayCommand(ExportToUIGFJsonAsync);
        RemoveArchiveCommand = new AsyncRelayCommand(RemoveArchiveAsync);
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
                SelectedHistoryWish = statistics?.HistoryWishes[0];
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
    /// 页面加载命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    /// <summary>
    /// 浏览器缓存刷新命令
    /// </summary>
    public ICommand RefreshByWebCacheCommand { get; }

    /// <summary>
    /// Stoken 刷新命令
    /// </summary>
    public ICommand RefreshByStokenCommand { get; }

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

    private async Task OpenUIAsync()
    {
        try
        {
            if (await gachaLogService.InitializeAsync(CancellationToken).ConfigureAwait(true))
            {
                ObservableCollection<GachaArchive> archives;

                ThrowIfViewDisposed();
                using (await DisposeLock.EnterAsync().ConfigureAwait(false))
                {
                    ThrowIfViewDisposed();
                    archives = await gachaLogService.GetArchiveCollectionAsync().ConfigureAwait(false);
                }

                await ThreadHelper.SwitchToMainThreadAsync();
                Archives = archives;
                SetSelectedArchiveAndUpdateStatistics(Archives.SingleOrDefault(a => a.IsSelected == true), true);
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

    private Task RefreshByStokenAsync()
    {
        return RefreshInternalAsync(RefreshOption.Stoken);
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
                IAsyncDisposable dialogHider = await dialog.BlockAsync().ConfigureAwait(false);
                Progress<FetchState> progress = new(dialog.OnReport);
                bool authkeyValid;

                try
                {
                    using (await DisposeLock.EnterAsync().ConfigureAwait(false))
                    {
                        try
                        {
                            authkeyValid = await gachaLogService.RefreshGachaLogAsync(query, strategy, progress, CancellationToken).ConfigureAwait(false);
                        }
                        catch (Core.ExceptionService.UserdataCorruptedException ex)
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
                    await dialogHider.DisposeAsync().ConfigureAwait(false);
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
                await gachaLogService.RemoveArchiveAsync(SelectedArchive).ConfigureAwait(false);

                // reselect first archive
                await ThreadHelper.SwitchToMainThreadAsync();
                SelectedArchive = Archives.FirstOrDefault();
            }
        }
    }

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
                // no gachalog
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
        GachaStatistics temp = await gachaLogService.GetStatisticsAsync(archive).ConfigureAwait(false);
        await ThreadHelper.SwitchToMainThreadAsync();
        Statistics = temp;
        IsInitialized = true;
    }

    private async Task<bool> TryImportUIGFInternalAsync(UIGF uigf)
    {
        if (uigf.IsCurrentVersionSupported())
        {
            // ContentDialog must be created by main thread.
            await ThreadHelper.SwitchToMainThreadAsync();
            if (await new GachaLogImportDialog(uigf).GetShouldImportAsync().ConfigureAwait(true))
            {
                ContentDialog dialog = await contentDialogFactory.CreateForIndeterminateProgressAsync(SH.ViewModelGachaLogImportProgress).ConfigureAwait(true);
                await using (await dialog.BlockAsync().ConfigureAwait(false))
                {
                    await gachaLogService.ImportFromUIGFAsync(uigf.List, uigf.Info.Uid).ConfigureAwait(false);
                }

                infoBarService.Success(SH.ViewModelGachaLogImportComplete);
                await ThreadHelper.SwitchToMainThreadAsync();
                SetSelectedArchiveAndUpdateStatistics(gachaLogService.CurrentArchive, true);
                return true;
            }
        }
        else
        {
            infoBarService.Warning(SH.ViewModelGachaLogImportWarningTitle, SH.ViewModelGachaLogImportWarningMessage);
        }

        return false;
    }
}