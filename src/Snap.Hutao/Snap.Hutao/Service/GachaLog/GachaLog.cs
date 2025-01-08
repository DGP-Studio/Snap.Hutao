// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.Frozen;

namespace Snap.Hutao.Service.GachaLog;

internal static class GachaLog
{
    public static readonly FrozenSet<GachaType> QueryTypes =
    [
        GachaType.NewBie,
        GachaType.Standard,
        GachaType.ActivityAvatar,
        GachaType.ActivityWeapon,
        GachaType.ActivityCity,
    ];
}