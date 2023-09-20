// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Calculable;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.AvatarProperty;

/// <summary>
/// 武器
/// </summary>
[HighQuality]
internal sealed class WeaponView : Equip, ICalculableSource<ICalculableWeapon>
{
    /// <summary>
    /// 副属性
    /// </summary>
    public NameDescription SubProperty { get; set; } = default!;

    /// <summary>
    /// 精炼等级
    /// </summary>
    public uint AffixLevelNumber { get; set; }

    /// <summary>
    /// 精炼属性
    /// </summary>
    public string AffixLevel { get => SH.ModelBindingAvatarPropertyWeaponAffixFormat.Format(AffixLevelNumber); }

    /// <summary>
    /// 精炼名称
    /// </summary>
    public string AffixName { get; set; } = default!;

    /// <summary>
    /// 精炼被动
    /// </summary>
    public string AffixDescription { get; set; } = default!;

    /// <summary>
    /// Id
    /// </summary>
    internal WeaponId Id { get; set; }

    /// <summary>
    /// 等级数字
    /// </summary>
    internal uint LevelNumber { get; set; }

    /// <summary>
    /// 最大等级
    /// </summary>
    internal uint MaxLevel { get => Model.Metadata.Weapon.Weapon.GetMaxLevelByQuality(Quality); }

    /// <inheritdoc/>
    public ICalculableWeapon ToCalculable()
    {
        return CalculableWeapon.From(this);
    }
}