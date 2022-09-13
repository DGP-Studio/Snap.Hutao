// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Control;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Binding;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.GachaLog;
using Snap.Hutao.Service.Metadata;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 祈愿记录视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal class GachaLogViewModel : ObservableObject, ISupportCancellation
{
    private readonly IMetadataService metadataService;
    private readonly IGachaLogService gachaLogService;
    private readonly IInfoBarService infoBarService;

    private ObservableCollection<GachaArchive>? archives;
    private GachaArchive? selectedArchive;
    private GachaStatistics? statistics;

    /// <summary>
    /// 构造一个新的祈愿记录视图模型
    /// </summary>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="gachaLogService">祈愿记录服务</param>
    /// <param name="infoBarService">信息</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    public GachaLogViewModel(
        IMetadataService metadataService,
        IGachaLogService gachaLogService,
        IInfoBarService infoBarService,
        IAsyncRelayCommandFactory asyncRelayCommandFactory)
    {
        this.metadataService = metadataService;
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
    /// </summary>
    public GachaArchive? SelectedArchive { get => selectedArchive; set => SetProperty(ref selectedArchive, value); }

    /// <summary>
    /// 当前统计信息
    /// </summary>
    public GachaStatistics? Statistics { get => statistics; set => SetProperty(ref statistics, value); }

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
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            Archives = gachaLogService.GetArchiveCollection();
            SelectedArchive = Archives.SingleOrDefault(a => a.IsSelected == true);

            if (SelectedArchive == null)
            {
                //infoBarService.Warning("请创建一个成就存档");
            }
        }
    }

    private async Task RefreshByWebCacheAsync()
    {
        Statistics = await gachaLogService.RefreshAsync();
    }

    private async Task RefreshByManualInputAsync()
    {

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
}