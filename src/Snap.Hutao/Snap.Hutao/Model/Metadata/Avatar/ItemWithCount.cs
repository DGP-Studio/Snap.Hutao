// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 带有个数的物品
/// </summary>
public class ItemWithCount
{
    /// <summary>
    /// 物品Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; } = default!;

    /// <summary>
    /// 物品等级
    /// </summary>
    public ItemQuality RankLevel { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Count { get; set; }
}