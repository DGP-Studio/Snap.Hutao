// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

/// <summary>
/// 物品与率
/// </summary>
/// <typeparam name="TItem">物品类型</typeparam>
/// <typeparam name="TRate">率类型</typeparam>
[HighQuality]
internal class ItemRate<TItem, TRate>
{
    /// <summary>
    /// 构造一个新的物品与率
    /// </summary>
    /// <param name="item">物品</param>
    /// <param name="rate">率</param>
    public ItemRate(TItem item, TRate rate)
    {
        Item = item;
        Rate = rate;
    }

    /// <summary>
    /// 物品
    /// </summary>
    public TItem Item { get; set; }

    /// <summary>
    /// 率
    /// </summary>
    public TRate Rate { get; set; }
}