// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Control;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Core.Threading.CodeAnalysis;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Binding.Gacha;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 祈愿记录视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal class GachaLogViewModel : ObservableObject, ISupportCancellation
{
    private readonly IGachaLogService gachaLogService;
    private readonly IInfoBarService infoBarService;

    private ObservableCollection<GachaArchive>? archives;
    private GachaArchive? selectedArchive;
    private GachaStatistics? statistics;
    private bool isAggressiveRefresh;

    /// <summary>
    /// 构造一个新的祈愿记录视图模型
    /// </summary>
    /// <param name="gachaLogService">祈愿记录服务</param>
    /// <param name="infoBarService">信息</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    public GachaLogViewModel(
        IGachaLogService gachaLogService,
        IInfoBarService infoBarService,
        IAsyncRelayCommandFactory asyncRelayCommandFactory)
    {
        this.gachaLogService = gachaLogService;
        this.infoBarService = infoBarService;

        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
        RefreshByWebCacheCommand = asyncRelayCommandFactory.Create(RefreshByWebCacheAsync);
        RefreshByManualInputCommand = asyncRelayCommandFactory.Create(RefreshByManualInputAsync);
        ImportFromUIGFExcelCommand = asyncRelayCommandFactory.Create(ImportFromUIGFExcelAsync);
        ImportFromUIGFJsonCommand = asyncRelayCommandFactory.Create(ImportFromUIGFJsonAsync);
        ExportToUIGFExcelCommand = asyncRelayCommandFactory.Create(ExportToUIGFExcelAsync);
        ExportToUIGFJsonCommand = asyncRelayCommandFactory.Create(ExportToUIGFJsonAsync);
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
                UpdateStatisticsAsync(selectedArchive).SafeForget();
            }
        }
    }

    /// <summary>
    /// 当前统计信息
    /// </summary>
    public GachaStatistics? Statistics { get => statistics; set => SetProperty(ref statistics, value); }

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

    private async Task OpenUIAsync()
    {
        if (await gachaLogService.InitializeAsync().ConfigureAwait(true))
        {
            Archives = gachaLogService.GetArchiveCollection();
            SelectedArchive = Archives.SingleOrDefault(a => a.IsSelected == true);

            if (SelectedArchive == null)
            {
                //infoBarService.Warning("请创建一个成就存档");
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

    }

    private async Task ImportFromUIGFJsonAsync()
    {

    }

    private async Task ExportToUIGFExcelAsync()
    {

    }

    private async Task ExportToUIGFJsonAsync()
    {

    }

    [ThreadAccess(ThreadAccessState.MainThread)]
    private async Task UpdateStatisticsAsync(GachaArchive? archive)
    {
        GachaStatistics temp = await gachaLogService.GetStatisticsAsync(archive).ConfigureAwait(false);
        await ThreadHelper.SwitchToMainThreadAsync();
        Statistics = temp;
    }
}