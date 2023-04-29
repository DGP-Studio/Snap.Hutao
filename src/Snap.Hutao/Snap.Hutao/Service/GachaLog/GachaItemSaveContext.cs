// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
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
    public readonly DbSet<GachaItem> GachaItems;

    /// <summary>
    /// 构造一个新的祈愿物品
    /// </summary>
    /// <param name="itemsToAdd">待添加物品</param>
    /// <param name="isLazy">是否懒惰</param>
    /// <param name="endId">结尾 Id</param>
    /// <param name="gachaItems">数据集</param>
    public GachaItemSaveContext(List<GachaItem> itemsToAdd, bool isLazy, long endId, DbSet<GachaItem> gachaItems)
    {
        ItemsToAdd = itemsToAdd;
        IsLazy = isLazy;
        EndId = endId;
        GachaItems = gachaItems;
    }
}