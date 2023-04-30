// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 祈愿配置类型比较器
/// </summary>
internal sealed class GachaConfigTypeComparar : IComparer<GachaConfigType>
{
    private static readonly GachaConfigTypeComparar InnerShared = new();
    private static readonly ImmutableDictionary<GachaConfigType, int> OrderMap = new Dictionary<GachaConfigType, int>()
    {
        [GachaConfigType.AvatarEventWish] = 0,
        [GachaConfigType.AvatarEventWish2] = 1,
        [GachaConfigType.WeaponEventWish] = 2,
        [GachaConfigType.StandardWish] = 3,
        [GachaConfigType.NoviceWish] = 4,
    }.ToImmutableDictionary();

    /// <summary>
    /// 共享的比较器
    /// </summary>
    public static GachaConfigTypeComparar Shared { get => InnerShared; }

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