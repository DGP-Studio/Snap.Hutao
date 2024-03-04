// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿物品保存上下文
/// </summary>
internal readonly struct GachaItemSaveContext
{
    /// <summary>
    /// 待添加物品
    /// </summary>
    public readonly List<GachaItem> ItemsToAdd;

    /// <summary>
    /// 是否懒惰
    /// </summary>
    public readonly bool IsLazy;

    public readonly GachaType QueryType;

    /// <summary>
    /// 结尾 Id
    /// </summary>
    public readonly long EndId;

    /// <summary>
    /// 数据集
    /// </summary>
    public readonly IGachaLogDbService GachaLogDbService;

    public GachaItemSaveContext(List<GachaItem> itemsToAdd, bool isLazy, GachaType queryType, long endId, IGachaLogDbService gachaLogDbService)
    {
        ItemsToAdd = itemsToAdd;
        IsLazy = isLazy;
        QueryType = queryType;
        EndId = endId;
        GachaLogDbService = gachaLogDbService;
    }

    public void SaveItems(GachaArchive archive)
    {
        if (ItemsToAdd.Count <= 0)
        {
            return;
        }

        // 全量刷新
        if (!IsLazy)
        {
            GachaLogDbService.RemoveNewerGachaItemRangeByArchiveIdQueryTypeAndEndId(archive.InnerId, QueryType, EndId);
        }

        GachaLogDbService.AddGachaItemRange(ItemsToAdd);
    }
}