// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Reliquary;
using Snap.Hutao.Model.Primitive;
using MetadataAvatar = Snap.Hutao.Model.Metadata.Avatar.Avatar;
using MetadataReliquary = Snap.Hutao.Model.Metadata.Reliquary.Reliquary;
using MetadataWeapon = Snap.Hutao.Model.Metadata.Weapon.Weapon;

namespace Snap.Hutao.Service.AvatarInfo.Factory;

/// <summary>
/// 简述元数据上下文
/// </summary>
[HighQuality]
internal sealed class SummaryMetadataContext
{
    /// <summary>
    /// 角色映射
    /// </summary>
    public Dictionary<AvatarId, MetadataAvatar> IdAvatarMap { get; set; } = default!;

    /// <summary>
    /// 武器映射
    /// </summary>
    public Dictionary<WeaponId, MetadataWeapon> IdWeaponMap { get; set; } = default!;

    /// <summary>
    /// 圣遗物主属性映射
    /// </summary>
    public Dictionary<ReliquaryMainAffixId, FightProperty> IdRelicMainPropMap { get; set; } = default!;

    /// <summary>
    /// 圣遗物副属性映射
    /// </summary>
    public Dictionary<ReliquaryAffixId, ReliquaryAffix> IdReliquaryAffixMap { get; set; } = default!;

    /// <summary>
    /// 圣遗物等级
    /// </summary>
    public List<ReliquaryLevel> ReliqueryLevels { get; set; } = default!;

    /// <summary>
    /// 圣遗物
    /// </summary>
    public List<MetadataReliquary> Reliquaries { get; set; } = default!;
}