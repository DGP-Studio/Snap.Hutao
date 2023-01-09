// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Binding.Gacha;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 祈愿记录视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class GachaLogViewModel : ObservableObject, ISupportCancellation
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
    private bool isInitialized;

    /// <summary>
    /// 构造一个新的祈愿记录视图模型
    /// </summary>
    /// <param name="gachaLogService">祈愿记录服务</param>
    /// <param name="infoBarService">信息</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    /// <param name="contentDialogFactory">内容对话框工厂</param>
    /// <param name="pickerFactory">文件选择器工厂</param>
    public GachaLogViewModel(
        IGachaLogService gachaLogService,
        IInfoBarService infoBarService,
        JsonSerializerOptions options,
        IAsyncRelayCommandFactory asyncRelayCommandFactory,
        IContentDialogFactory contentDialogFactory,
        IPickerFactory pickerFactory)
    {
        this.gachaLogService = gachaLogService;
        this.infoBarService = infoBarService;
        this.pickerFactory = pickerFactory;
        this.contentDialogFactory = contentDialogFactory;
        this.options = options;

        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
        RefreshByWebCacheCommand = asyncRelayCommandFactory.Create(RefreshByWebCacheAsync);
        RefreshByStokenCommand = asyncRelayCommandFactory.Create(RefreshByStokenAsync);
        RefreshByManualInputCommand = asyncRelayCommandFactory.Create(RefreshByManualInputAsync);
        ImportFromUIGFJsonCommand = asyncRelayCommandFactory.Create(ImportFromUIGFJsonAsync);
        ExportToUIGFJsonCommand = asyncRelayCommandFactory.Create(ExportToUIGFJsonAsync);
        RemoveArchiveCommand = asyncRelayCommandFactory.Create(RemoveArchiveAsync);
    }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

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
        set => SetSelectedArchiveAndUpdateStatistics(value, false);
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
    /// 初始化是否完成
    /// </summary>
    public bool IsInitialized { get => isInitialized; set => SetProperty(ref isInitialized, value); }

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
        if (await gachaLogService.InitializeAsync().ConfigureAwait(true))
        {
            Archives = gachaLogService.GetArchiveCollection();
            SelectedArchive = Archives.SingleOrDefault(a => a.IsSelected == true);

            await ThreadHelper.SwitchToMainThreadAsync();
            IsInitialized = true;
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
        IGachaLogUrlProvider? provider = gachaLogService.GetGachaLogUrlProvider(option);

        if (provider != null)
        {
            (bool isOk, string query) = await provider.GetQueryAsync().ConfigureAwait(false);

            if (isOk)
            {
                RefreshStrategy strategy = IsAggressiveRefresh ? RefreshStrategy.AggressiveMerge : RefreshStrategy.LazyMerge;

                GachaLogRefreshProgressDialog dialog = new();
                IAsyncDisposable dialogHider = await dialog.BlockAsync().ConfigureAwait(false);
                Progress<FetchState> progress = new(dialog.OnReport);
                bool authkeyValid = await gachaLogService.RefreshGachaLogAsync(query, strategy, progress, default).ConfigureAwait(false);

                await ThreadHelper.SwitchToMainThreadAsync();
                if (authkeyValid)
                {
                    SetSelectedArchiveAndUpdateStatistics(gachaLogService.CurrentArchive, true);
                    await dialogHider.DisposeAsync().ConfigureAwait(false);
                }
                else
                {
                    dialog.Title = "获取祈愿记录失败";
                    dialog.PrimaryButtonText = "确认";
                    dialog.DefaultButton = ContentDialogButton.Primary;
                }
            }
            else
            {
                if (query is string message)
                {
                    infoBarService.Warning(message);
                }
            }
        }
    }

    private async Task ImportFromUIGFJsonAsync()
    {
        FileOpenPicker picker = pickerFactory.GetFileOpenPicker(PickerLocationId.Desktop, "导入", ".json");

        if (await picker.PickSingleFileAsync() is StorageFile file)
        {
            (bool isOk, UIGF? uigf) = await file.DeserializeFromJsonAsync<UIGF>(options).ConfigureAwait(false);
            if (isOk)
            {
                Must.NotNull(uigf!);
                await TryImportUIGFInternalAsync(uigf).ConfigureAwait(false);
            }
            else
            {
                await ThreadHelper.SwitchToMainThreadAsync();
                await contentDialogFactory.CreateForConfirm("导入失败", "文件的数据格式不正确").ShowAsync();
            }
        }
    }

    private async Task ExportToUIGFJsonAsync()
    {
        if (SelectedArchive == null)
        {
            return;
        }

        FileSavePicker picker = pickerFactory.GetFileSavePicker();
        picker.SuggestedStartLocation = PickerLocationId.Desktop;
        picker.SuggestedFileName = SelectedArchive.Uid;
        picker.CommitButtonText = "导出";
        picker.FileTypeChoices.Add("UIGF Json 文件", new List<string>() { ".json" });

        if (await picker.PickSaveFileAsync() is StorageFile file)
        {
            UIGF uigf = await gachaLogService.ExportToUIGFAsync(SelectedArchive).ConfigureAwait(false);
            bool isOk = await file.SerializeToJsonAsync(uigf, options).ConfigureAwait(false);

            await ThreadHelper.SwitchToMainThreadAsync();
            if (isOk)
            {
                await contentDialogFactory.CreateForConfirm("导出成功", "成功保存到指定位置").ShowAsync();
            }
            else
            {
                await contentDialogFactory.CreateForConfirm("导出失败", "写入文件时遇到问题").ShowAsync();
            }
        }
    }

    private async Task RemoveArchiveAsync()
    {
        if (Archives != null && SelectedArchive != null)
        {
            ContentDialogResult result = await contentDialogFactory
                .CreateForConfirmCancel($"确定要删除存档 {SelectedArchive.Uid} 吗？", "该操作是不可逆的，该存档和其内的所有祈愿数据会丢失。")
                .ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await gachaLogService.RemoveArchiveAsync(SelectedArchive).ConfigureAwait(false);

                // reselect first archive
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

        if (changed || forceUpdate)
        {
            if (archive != null)
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
    }

    private async Task<bool> TryImportUIGFInternalAsync(UIGF uigf)
    {
        if (uigf.IsCurrentVersionSupported())
        {
            if (await new GachaLogImportDialog(uigf).GetShouldImportAsync().ConfigureAwait(true))
            {
                await using (await contentDialogFactory.CreateForIndeterminateProgress("导入祈愿记录中").BlockAsync().ConfigureAwait(false))
                {
                    await gachaLogService.ImportFromUIGFAsync(uigf.List, uigf.Info.Uid).ConfigureAwait(false);
                }

                infoBarService.Success($"导入完成");
                await ThreadHelper.SwitchToMainThreadAsync();
                SetSelectedArchiveAndUpdateStatistics(gachaLogService.CurrentArchive, true);
                return true;
            }
        }
        else
        {
            await ThreadHelper.SwitchToMainThreadAsync();
            await contentDialogFactory.CreateForConfirm("导入失败", "数据的 UIGF 版本过低，无法导入").ShowAsync();
        }

        return false;
    }
}