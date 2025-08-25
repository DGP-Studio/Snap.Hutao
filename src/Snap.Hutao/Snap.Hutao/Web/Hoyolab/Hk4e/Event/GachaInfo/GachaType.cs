// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

[ExtendedEnum]
internal enum GachaType
{
    [LocalizationKey(nameof(SH.WebGachaConfigTypeNoviceWish))]
    NewBie = 100,

    [LocalizationKey(nameof(SH.WebGachaConfigTypePermanentWish))]
    Standard = 200,

    [LocalizationKey(nameof(SH.WebGachaConfigTypeAvatarEventWish))]
    ActivityAvatar = 301,

    [LocalizationKey(nameof(SH.WebGachaConfigTypeWeaponEventWish))]
    ActivityWeapon = 302,

    [LocalizationKey(nameof(SH.WebGachaConfigTypeAvatarEventWish2))]
    SpecialActivityAvatar = 400,

    [LocalizationKey(nameof(SH.WebGachaConfigTypeChronicledWish))]
    ActivityCity = 500,
}