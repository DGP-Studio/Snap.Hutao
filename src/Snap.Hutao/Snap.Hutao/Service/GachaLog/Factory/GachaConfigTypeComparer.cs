// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.Frozen;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 祈愿配置类型比较器
/// </summary>
internal sealed class GachaConfigTypeComparer : IComparer<GachaConfigType>
{
    private static readonly Lazy<GachaConfigTypeComparer> LazyShared = new(() => new());
    private static readonly FrozenDictionary<GachaConfigType, int> OrderMap = new Dictionary<GachaConfigType, int>()
    {
        [GachaConfigType.AvatarEventWish] = 0,
        [GachaConfigType.AvatarEventWish2] = 1,
        [GachaConfigType.WeaponEventWish] = 2,
        [GachaConfigType.StandardWish] = 3,
        [GachaConfigType.NoviceWish] = 4,
    }.ToFrozenDictionary();

    /// <summary>
    /// 共享的比较器
    /// </summary>
    public static GachaConfigTypeComparer Shared { get => LazyShared.Value; }

    /// <inheritdoc/>
    public int Compare(GachaConfigType x, GachaConfigType y)
    {
        return OrderOf(x) - OrderOf(y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int OrderOf(GachaConfigType type)
    {
        return OrderMap.GetValueOrDefault(type, 0);
    }
}