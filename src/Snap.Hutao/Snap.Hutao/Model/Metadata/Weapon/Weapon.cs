// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Weapon;

/// <summary>
/// 武器
/// </summary>
[HighQuality]
internal sealed partial class Weapon
{
    /// <summary>
    /// Id
    /// </summary>
    public WeaponId Id { get; set; }

    /// <summary>
    /// 突破 Id
    /// </summary>
    public PromoteId PromoteId { get; set; }

    /// <summary>
    /// 武器类型
    /// </summary>
    public WeaponType WeaponType { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    public QualityType RankLevel { get; set; }

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

    /// <summary>
    /// 觉醒图标
    /// </summary>
    public string AwakenIcon { get; set; } = default!;

    /// <summary>
    /// 生长曲线
    /// </summary>
    public List<WeaponTypeValue> GrowCurves { get; set; } = default!;

    /// <summary>
    /// 被动信息, 无被动的武器为 <see langword="null"/>
    /// </summary>
    public NameDescriptions? Affix { get; set; } = default!;

    /// <summary>
    /// 养成物品
    /// </summary>
    public List<MaterialId> CultivationItems { get; set; } = default!;
}