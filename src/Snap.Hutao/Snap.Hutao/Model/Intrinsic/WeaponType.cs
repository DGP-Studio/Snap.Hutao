// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

[ExtendedEnum]
internal enum WeaponType
{
    /// <summary>
    /// ?
    /// </summary>
    WEAPON_NONE = 0,

    /// <summary>
    /// 单手剑
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicWeaponTypeSwordOneHand))]
    WEAPON_SWORD_ONE_HAND = 1,

    /// <summary>
    /// 法器
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicWeaponTypeCatalyst))]
    WEAPON_CATALYST = 10,

    /// <summary>
    /// 双手剑
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicWeaponTypeClaymore))]
    WEAPON_CLAYMORE = 11,

    /// <summary>
    /// 弓
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicWeaponTypeBow))]
    WEAPON_BOW = 12,

    /// <summary>
    /// 长柄武器
    /// </summary>
    [LocalizationKey(nameof(SH.ModelIntrinsicWeaponTypePole))]
    WEAPON_POLE = 13,
}