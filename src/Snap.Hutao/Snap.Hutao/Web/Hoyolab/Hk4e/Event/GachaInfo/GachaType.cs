// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

/// <summary>
/// 祈愿类型
/// </summary>
[HighQuality]
[Localization]
internal enum GachaType
{
    /// <summary>
    /// 新手池
    /// </summary>
    [LocalizationKey(nameof(SH.WebGachaConfigTypeNoviceWish))]
    NewBie = 100,

    /// <summary>
    /// 常驻池
    /// </summary>
    [LocalizationKey(nameof(SH.WebGachaConfigTypePermanentWish))]
    Standard = 200,

    /// <summary>
    /// 角色1池
    /// </summary>
    [LocalizationKey(nameof(SH.WebGachaConfigTypeAvatarEventWish))]
    ActivityAvatar = 301,

    /// <summary>
    /// 武器池
    /// </summary>
    [LocalizationKey(nameof(SH.WebGachaConfigTypeWeaponEventWish))]
    ActivityWeapon = 302,

    /// <summary>
    /// 角色2池
    /// </summary>
    [LocalizationKey(nameof(SH.WebGachaConfigTypeAvatarEventWish2))]
    SpecialActivityAvatar = 400,

    /// <summary>
    /// 集录池
    /// </summary>
    [LocalizationKey(nameof(SH.WebGachaConfigTypeChronicledWish))]
    ActivityCity = 500,
}