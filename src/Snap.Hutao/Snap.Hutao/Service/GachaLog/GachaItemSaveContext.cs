// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;

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

    /// <summary>
    /// 结尾 Id
    /// </summary>
    public readonly long EndId;

    /// <summary>
    /// 数据集
    /// </summary>
    public readonly IGachaLogDbService GachaLogDbService;

    public GachaItemSaveContext(List<GachaItem> itemsToAdd, bool isLazy, long endId, IGachaLogDbService gachaLogDbService)
    {
        ItemsToAdd = itemsToAdd;
        IsLazy = isLazy;
        EndId = endId;
        GachaLogDbService = gachaLogDbService;
    }

    public void SaveItems(GachaArchive archive)
    {
        if (ItemsToAdd.Count > 0)
        {
            // 全量刷新
            if (!IsLazy)
            {
                GachaLogDbService.DeleteNewerGachaItemsByArchiveIdAndEndId(archive.InnerId, EndId);
            }

            GachaLogDbService.AddGachaItems(ItemsToAdd);
        }
    }
}