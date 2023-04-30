// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Reliquary;

/// <summary>
/// 圣遗物信息
/// </summary>
[HighQuality]
internal sealed class Reliquary
{
    /// <summary>
    /// 表示同种类的Id
    /// </summary>
    public List<ReliquaryId> Ids { get; set; } = default!;

    /// <summary>
    /// 允许出现的等级
    /// </summary>
    public ItemQuality RankLevel { get; set; } = default!;

    /// <summary>
    /// 套装Id
    /// </summary>
    public ReliquarySetId SetId { get; set; }

    /// <summary>
    /// 装备类型
    /// </summary>
    public EquipType EquipType { get; set; } = default!;

    /// <summary>
    /// 物品类型
    /// </summary>
    public ItemType ItemType { get; set; } = default!;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; } = default!;
}