// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Reliquary;

/// <summary>
/// 圣遗物套装
/// </summary>
[HighQuality]
internal sealed class ReliquarySet
{
    /// <summary>
    /// 套装Id
    /// </summary>
    public ReliquarySetId SetId { get; set; }

    /// <summary>
    /// 装备被动Id
    /// </summary>
    public EquipAffixId EquipAffixId { get; set; }

    /// <summary>
    /// 装备被动的被动Id
    /// </summary>
    public HashSet<ExtendedEquipAffixId> EquipAffixIds { get; set; } = default!;

    /// <summary>
    /// 套装名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 套装图标
    /// </summary>
    public string Icon { get; set; } = default!;

    /// <summary>
    /// 需要的数量
    /// </summary>
    public List<int> NeedNumber { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public List<string> Descriptions { get; set; } = default!;
}