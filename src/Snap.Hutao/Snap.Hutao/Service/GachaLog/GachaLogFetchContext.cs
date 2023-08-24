// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.ObjectModel;

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

    private readonly GachaLogServiceMetadataContext serviceContext;
    private readonly IGachaLogDbService gachaLogDbService;
    private readonly ITaskContext taskContext;
    private readonly bool isLazy;

    public GachaLogFetchContext(IGachaLogDbService gachaLogDbService, ITaskContext taskContext, in GachaLogServiceMetadataContext serviceContext, bool isLazy)
    {
        this.gachaLogDbService = gachaLogDbService;
        this.taskContext = taskContext;
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
    public void ResetForProcessingPage()
    {
        FetchStatus = new(CurrentType);
        CurrentTypeAddingCompleted = false;
    }

    /// <summary>
    /// 确保 存档 与 EndId 不为空
    /// </summary>
    /// <param name="item">物品</param>
    /// <param name="archives">存档集合</param>
    /// <param name="gachaLogDbService">祈愿记录数据库服务</param>
    public void EnsureArchiveAndEndId(GachaLogItem item, ObservableCollection<GachaArchive> archives, IGachaLogDbService gachaLogDbService)
    {
        if (TargetArchive is null)
        {
            GachaArchiveOperation.GetOrAdd(gachaLogDbService, taskContext, item.Uid, archives, out TargetArchive);
        }

        DbEndId ??= gachaLogDbService.GetNewestGachaItemIdByArchiveIdAndQueryType(TargetArchive.InnerId, CurrentType);
    }

    /// <summary>
    /// 判断是否应添加
    /// </summary>
    /// <param name="item">物品</param>
    /// <returns>是否应添加</returns>
    public readonly bool ShouldAddItem(GachaLogItem item)
    {
        return !isLazy || item.Id > DbEndId;
    }

    /// <summary>
    /// 判断当前类型已经处理完成
    /// </summary>
    /// <param name="items">物品集合</param>
    /// <returns>当前类型已经处理完成</returns>
    public readonly bool ItemsHaveReachEnd(List<GachaLogItem> items)
    {
        return CurrentTypeAddingCompleted || items.Count < GachaLogQueryOptions.Size;
    }

    /// <summary>
    /// 添加物品
    /// </summary>
    /// <param name="item">物品</param>
    public void AddItem(GachaLogItem item)
    {
        ArgumentNullException.ThrowIfNull(TargetArchive);
        ItemsToAdd.Add(GachaItem.From(TargetArchive.InnerId, item, serviceContext.GetItemId(item)));
        FetchStatus.Items.Add(serviceContext.GetItemByNameAndType(item.Name, item.ItemType));
        QueryOptions.EndId = item.Id;
    }

    /// <summary>
    /// 保存物品
    /// </summary>
    public readonly void SaveItems()
    {
        // While no item is fetched, archive can be null.
        if (TargetArchive is not null)
        {
            GachaItemSaveContext saveContext = new(ItemsToAdd, isLazy, QueryOptions.EndId, gachaLogDbService);
            saveContext.SaveItems(TargetArchive);
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
    public readonly void Report(IProgress<GachaLogFetchStatus> progress, bool isAuthKeyTimeout = false)
    {
        FetchStatus.AuthKeyTimeout = isAuthKeyTimeout;
        progress.Report(FetchStatus);
    }
}