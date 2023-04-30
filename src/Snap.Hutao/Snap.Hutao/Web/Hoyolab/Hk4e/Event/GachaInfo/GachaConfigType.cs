// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

/// <summary>
/// 祈愿配置类型
/// </summary>
[HighQuality]
[Localization]
internal enum GachaConfigType
{
    /// <summary>
    /// 新手池
    /// </summary>
    [Description("新手祈愿")]
    [LocalizationKey("WebGachaConfigTypeNoviceWish")]
    NoviceWish = 100,

    /// <summary>
    /// 常驻池
    /// </summary>
    [Description("常驻祈愿")]
    [LocalizationKey("WebGachaConfigTypePermanentWish")]
    StandardWish = 200,

    /// <summary>
    /// 角色1池
    /// </summary>
    [Description("角色活动祈愿")]
    [LocalizationKey("WebGachaConfigTypeAvatarEventWish")]
    AvatarEventWish = 301,

    /// <summary>
    /// 武器池
    /// </summary>
    [Description("武器活动祈愿")]
    [LocalizationKey("WebGachaConfigTypeWeaponEventWish")]
    WeaponEventWish = 302,

    /// <summary>
    /// 角色2池
    /// </summary>
    [Description("角色活动祈愿-2")]
    [LocalizationKey("WebGachaConfigTypeAvatarEventWish2")]
    AvatarEventWish2 = 400,
}