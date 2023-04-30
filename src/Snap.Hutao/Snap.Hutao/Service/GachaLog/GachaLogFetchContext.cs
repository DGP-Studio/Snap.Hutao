// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录获取上下文
/// </summary>
internal struct GachaLogFetchContext
{
    /// <summary>
    /// 当前处理的存档
    /// </summary>
    public GachaArchive? TargetArchive;

    /// <summary>
    /// 当前的获取状态
    /// </summary>
    public GachaLogFetchStatus FetchStatus = default!;

    /// <summary>
    /// 当前的数据库 End Id
    /// </summary>
    public long? DbEndId;

    /// <summary>
    /// 查询选项
    /// </summary>
    public GachaLogQueryOptions QueryOptions;

    /// <summary>
    /// 待加入数据库的物品
    /// </summary>
    public List<GachaItem> ItemsToAdd = default!;

    /// <summary>
    /// 当前类型增加物品是否结束
    /// </summary>
    public bool CurrentTypeAddingCompleted;

    /// <summary>
    /// 当前类型
    /// </summary>
    public GachaConfigType CurrentType;

    private readonly IServiceProvider serviceProvider;
    private readonly GachaLogServiceContext serviceContext;
    private readonly bool isLazy;

    /// <summary>
    /// 构造一个新的祈愿记录获取上下文
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="serviceContext">祈愿服务上下文</param>
    /// <param name="isLazy">是否为懒惰模式</param>
    public GachaLogFetchContext(IServiceProvider serviceProvider, in GachaLogServiceContext serviceContext, bool isLazy)
    {
        this.serviceProvider = serviceProvider;
        this.serviceContext = serviceContext;
        this.isLazy = isLazy;
    }

    /// <summary>
    /// 为下一个卡池类型重置
    /// </summary>
    /// <param name="configType">卡池类型</param>
    /// <param name="query">查询</param>
    public void ResetForProcessingType(GachaConfigType configType, in GachaLogQuery query)
    {
        DbEndId = null;
        CurrentType = configType;
        ItemsToAdd = new();
        FetchStatus = new(configType);
        QueryOptions = new(query, configType);
    }

    /// <summary>
    /// 为下一个物品页面重置
    /// </summary>
    /// <param name="configType">卡池类型</param>
    public void ResetForProcessingPage()
    {
        FetchStatus = new(CurrentType);
        CurrentTypeAddingCompleted = false;
    }

    /// <summary>
    /// 确保 存档 与 EndId 不为空
    /// </summary>
    /// <param name="item">物品</param>
    public void EnsureArchiveAndEndId(GachaLogItem item)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            ITaskContext taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();

            GachaArchiveInitializationContext initContext = new(taskContext, item.Uid, appDbContext.GachaArchives, serviceContext.ArchiveCollection);
            GachaArchive.SkipOrInit(initContext, ref TargetArchive);
            DbEndId ??= TargetArchive.GetEndId(CurrentType, appDbContext.GachaItems);
        }
    }

    /// <summary>
    /// 判断是否应添加
    /// </summary>
    /// <param name="item">物品</param>
    /// <returns>是否应添加</returns>
    public bool ShouldAdd(GachaLogItem item)
    {
        return !isLazy || item.Id > DbEndId;
    }

    /// <summary>
    /// 判断当前类型已经处理完成
    /// </summary>
    /// <param name="items">物品集合</param>
    /// <returns>当前类型已经处理完成</returns>
    public bool ItemsHaveReachEnd(List<GachaLogItem> items)
    {
        return CurrentTypeAddingCompleted || items.Count < GachaLogQueryOptions.Size;
    }

    /// <summary>
    /// 添加物品
    /// </summary>
    /// <param name="item">物品</param>
    public void AddItem(GachaLogItem item)
    {
        ItemsToAdd.Add(GachaItem.Create(TargetArchive!.InnerId, item, serviceContext.GetItemId(item)));
        FetchStatus.Items.Add(serviceContext.GetItemByNameAndType(item.Name, item.ItemType));
        QueryOptions.EndId = item.Id;
    }

    /// <summary>
    /// 保存物品
    /// </summary>
    public void SaveItems()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            GachaItemSaveContext saveContext = new(ItemsToAdd, isLazy, QueryOptions.EndId, appDbContext.GachaItems);
            TargetArchive!.SaveItems(saveContext);
        }
    }

    /// <summary>
    /// 完成添加
    /// </summary>
    public void CompleteAdding()
    {
        CurrentTypeAddingCompleted = true;
    }

    /// <summary>
    /// 反馈进度
    /// </summary>
    /// <param name="progress">进度</param>
    /// <param name="isAuthKeyTimeout">验证密钥是否过期</param>
    public void Report(IProgress<GachaLogFetchStatus> progress, bool isAuthKeyTimeout = false)
    {
        FetchStatus.AuthKeyTimeout = isAuthKeyTimeout;
        progress.Report(FetchStatus);
    }
}