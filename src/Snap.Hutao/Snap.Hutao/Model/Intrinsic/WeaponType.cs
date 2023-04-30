// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Intrinsic;

/// <summary>
/// 武器类型
/// </summary>
[HighQuality]
[Localization]
[SuppressMessage("", "SA1124")]
internal enum WeaponType
{
    /// <summary>
    /// ?
    /// </summary>
    WEAPON_NONE = 0,

    /// <summary>
    /// 单手剑
    /// </summary>
    [LocalizationKey("ModelIntrinsicWeaponTypeSwordOneHand")]
    WEAPON_SWORD_ONE_HAND = 1,

    #region Not Used

    /// <summary>
    /// ?
    /// </summary>
    [Obsolete("尚未发现使用")]
    WEAPON_CROSSBOW = 2,

    /// <summary>
    /// ?
    /// </summary>
    [Obsolete("尚未发现使用")]
    WEAPON_STAFF = 3,

    /// <summary>
    /// ?
    /// </summary>
    [Obsolete("尚未发现使用")]
    WEAPON_DOUBLE_DAGGER = 4,

    /// <summary>
    /// ?
    /// </summary>
    [Obsolete("尚未发现使用")]
    WEAPON_KATANA = 5,

    /// <summary>
    /// ?
    /// </summary>
    [Obsolete("尚未发现使用")]
    WEAPON_SHURIKEN = 6,

    /// <summary>
    /// ?
    /// </summary>
    [Obsolete("尚未发现使用")]
    WEAPON_STICK = 7,

    /// <summary>
    /// ?
    /// </summary>
    [Obsolete("尚未发现使用")]
    WEAPON_SPEAR = 8,

    /// <summary>
    /// ?
    /// </summary>
    [Obsolete("尚未发现使用")]
    WEAPON_SHIELD_SMALL = 9,
    #endregion

    /// <summary>
    /// 法器
    /// </summary>
    [LocalizationKey("ModelIntrinsicWeaponTypeCatalyst")]
    WEAPON_CATALYST = 10,

    /// <summary>
    /// 双手剑
    /// </summary>
    [LocalizationKey("ModelIntrinsicWeaponTypeClaymore")]
    WEAPON_CLAYMORE = 11,

    /// <summary>
    /// 弓
    /// </summary>
    [LocalizationKey("ModelIntrinsicWeaponTypeBow")]
    WEAPON_BOW = 12,

    /// <summary>
    /// 长柄武器
    /// </summary>
    [LocalizationKey("ModelIntrinsicWeaponTypePole")]
    WEAPON_POLE = 13,
}