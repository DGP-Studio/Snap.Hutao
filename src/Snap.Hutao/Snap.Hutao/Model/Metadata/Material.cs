// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata;

/// <summary>
/// 材料
/// </summary>
public class Material
{
    /// <summary>
    /// 物品Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    public ItemQuality RankLevel { get; set; }

    /// <summary>
    /// 物品类型
    /// </summary>
    public ItemType ItemType { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; } = default!;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// 类型描述
    /// </summary>
    public string TypeDescription { get; set; } = default!;
}