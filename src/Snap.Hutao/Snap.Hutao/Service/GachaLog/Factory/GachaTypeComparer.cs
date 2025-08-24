// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.Frozen;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.GachaLog.Factory;

internal sealed class GachaTypeComparer : IComparer<GachaType>
{
    private static readonly Lazy<GachaTypeComparer> LazyShared = new(() => new());
    private static readonly FrozenDictionary<GachaType, int> OrderMap = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(GachaType.ActivityAvatar, 0),
        KeyValuePair.Create(GachaType.SpecialActivityAvatar, 1),
        KeyValuePair.Create(GachaType.ActivityWeapon, 2),
        KeyValuePair.Create(GachaType.ActivityCity, 3),
        KeyValuePair.Create(GachaType.Standard, 4),
        KeyValuePair.Create(GachaType.NewBie, 5),
    ]);

    public static GachaTypeComparer Shared { get => LazyShared.Value; }

    public int Compare(GachaType x, GachaType y)
    {
        return OrderOf(x) - OrderOf(y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int OrderOf(GachaType type)
    {
        return OrderMap.GetValueOrDefault(type, 0);
    }
}