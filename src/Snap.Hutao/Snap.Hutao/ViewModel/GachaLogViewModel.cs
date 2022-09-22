// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core.IO;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Core.Threading.CodeAnalysis;
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
[Injection(InjectAs.Transient)]
internal class GachaLogViewModel : ObservableObject, ISupportCancellation
{
    private readonly IGachaLogService gachaLogService;
    private readonly IInfoBarService infoBarService;
    private readonly IPickerFactory pickerFactory;
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
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    /// <param name="pickerFactory">文件选择器工厂</param>
    public GachaLogViewModel(
        IGachaLogService gachaLogService,
        IInfoBarService infoBarService,
        JsonSerializerOptions options,
        IAsyncRelayCommandFactory asyncRelayCommandFactory,
        IPickerFactory pickerFactory)
    {
        this.gachaLogService = gachaLogService;
        this.infoBarService = infoBarService;
        this.pickerFactory = pickerFactory;
        this.options = options;

        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
        RefreshByWebCacheCommand = asyncRelayCommandFactory.Create(RefreshByWebCacheAsync);
        RefreshByManualInputCommand = asyncRelayCommandFactory.Create(RefreshByManualInputAsync);
        ImportFromUIGFExcelCommand = asyncRelayCommandFactory.Create(ImportFromUIGFExcelAsync);
        ImportFromUIGFJsonCommand = asyncRelayCommandFactory.Create(ImportFromUIGFJsonAsync);
        ExportToUIGFExcelCommand = asyncRelayCommandFactory.Create(ExportToUIGFExcelAsync);
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
        set
        {
            if (SetProperty(ref selectedArchive, value))
            {
                gachaLogService.CurrentArchive = selectedArchive;
                if (selectedArchive != null)
                {
                    UpdateStatisticsAsync(selectedArchive).SafeForget();
                }
            }
        }
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
    /// 手动输入Url刷新命令
    /// </summary>
    public ICommand RefreshByManualInputCommand { get; }

    /// <summary>
    /// 从 UIGF Excel 导入命令
    /// </summary>
    public ICommand ImportFromUIGFExcelCommand { get; }

    /// <summary>
    /// 从 UIGF Json 导入命令
    /// </summary>
    public ICommand ImportFromUIGFJsonCommand { get; }

    /// <summary>
    /// 导出到 UIGF Excel 命令
    /// </summary>
    public ICommand ExportToUIGFExcelCommand { get; }

    /// <summary>
    /// 导出到 UIGF Json 命令
    /// </summary>
    public ICommand ExportToUIGFJsonCommand { get; }

    /// <summary>
    /// 删除存档命令
    /// </summary>
    public ICommand RemoveArchiveCommand { get; }

    [ThreadAccess(ThreadAccessState.MainThread)]
    private static Task<ContentDialogResult> ShowImportFailDialogAsync(string message)
    {
        ContentDialog dialog = new()
        {
            Title = "导入失败",
            Content = message,
            PrimaryButtonText = "确认",
            DefaultButton = ContentDialogButton.Primary,
        };

        MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
        return dialog.InitializeWithWindow(mainWindow).ShowAsync().AsTask();
    }

    private async Task OpenUIAsync()
    {
        if (await gachaLogService.InitializeAsync().ConfigureAwait(true))
        {
            Archives = gachaLogService.GetArchiveCollection();
            SelectedArchive = Archives.SingleOrDefault(a => a.IsSelected == true);

            if (SelectedArchive == null)
            {
                infoBarService.Information("请刷新或导入祈愿记录");
            }
        }
    }

    private Task RefreshByWebCacheAsync()
    {
        return RefreshInternalAsync(RefreshOption.WebCache);
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

                MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
                GachaLogRefreshProgressDialog dialog = new(mainWindow);
                await using (await dialog.BlockAsync().ConfigureAwait(false))
                {
                    Progress<FetchState> progress = new(dialog.OnReport);
                    await gachaLogService.RefreshGachaLogAsync(query, strategy, progress, default).ConfigureAwait(false);

                    await ThreadHelper.SwitchToMainThreadAsync();
                    SelectedArchive = gachaLogService.CurrentArchive;
                }
            }
        }
    }

    private async Task ImportFromUIGFExcelAsync()
    {
        await Task.Yield();
    }

    private async Task ImportFromUIGFJsonAsync()
    {
        FileOpenPicker picker = pickerFactory.GetFileOpenPicker();
        picker.SuggestedStartLocation = PickerLocationId.Desktop;
        picker.FileTypeFilter.Add(".json");

        if (await picker.PickSingleFileAsync() is StorageFile file)
        {
            if (await file.DeserializeJsonAsync<UIGF>(options, ex => infoBarService?.Error(ex)).ConfigureAwait(false) is UIGF uigf)
            {
                await TryImportUIGFInternalAsync(uigf).ConfigureAwait(false);
            }
            else
            {
                await ThreadHelper.SwitchToMainThreadAsync();
                await ShowImportFailDialogAsync("数据格式不正确").ConfigureAwait(false);
            }
        }
    }

    private async Task ExportToUIGFExcelAsync()
    {
        await Task.Yield();
    }

    private async Task ExportToUIGFJsonAsync()
    {
        await Task.Yield();
    }

    private async Task RemoveArchiveAsync()
    {
        if (Archives != null && SelectedArchive != null)
        {
            ContentDialog dialog = new()
            {
                Title = $"确定要删除存档 {SelectedArchive.Uid} 吗？",
                Content = "该操作是不可逆的，该存档和其内的所有祈愿数据会丢失。",
                PrimaryButtonText = "确认",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Close,
            };

            MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
            ContentDialogResult result = await dialog.InitializeWithWindow(mainWindow).ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await gachaLogService.RemoveArchiveAsync(SelectedArchive).ConfigureAwait(false);

                // reselect first archive
                SelectedArchive = Archives.FirstOrDefault();
            }
        }
    }

    [ThreadAccess(ThreadAccessState.MainThread)]
    private async Task UpdateStatisticsAsync(GachaArchive? archive)
    {
        GachaStatistics temp = await gachaLogService.GetStatisticsAsync(archive).ConfigureAwait(false);
        await ThreadHelper.SwitchToMainThreadAsync();
        Statistics = temp;
    }

    [ThreadAccess(ThreadAccessState.AnyThread)]
    private async Task<bool> TryImportUIGFInternalAsync(UIGF uigf)
    {
        if (uigf.IsCurrentVersionSupported())
        {
            MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
            await ThreadHelper.SwitchToMainThreadAsync();
            if (await new GachaLogImportDialog(mainWindow, uigf).GetShouldImportAsync().ConfigureAwait(true))
            {
                ContentDialog importingDialog = new()
                {
                    Title = "导入祈愿记录中",
                    Content = new ProgressBar() { IsIndeterminate = true },
                };

                await using (await importingDialog.InitializeWithWindow(mainWindow).BlockAsync().ConfigureAwait(false))
                {
                    await gachaLogService.ImportFromUIGFAsync(uigf.List, uigf.Info.Uid).ConfigureAwait(false);
                }

                infoBarService.Success($"导入完成");
                await ThreadHelper.SwitchToMainThreadAsync();
                SelectedArchive = gachaLogService.CurrentArchive;
                return true;
            }
        }
        else
        {
            await ThreadHelper.SwitchToMainThreadAsync();
            await ShowImportFailDialogAsync("数据的 UIGF 版本过低，无法导入").ConfigureAwait(false);
        }

        return false;
    }
}