// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 祈愿配置类型比较器
/// </summary>
public class GachaConfigTypeComparar : IComparer<GachaConfigType>
{
    /// <inheritdoc/>
    public int Compare(GachaConfigType x, GachaConfigType y)
    {
        return GetOrder(x) - GetOrder(y);
    }

    private static int GetOrder(GachaConfigType type)
    {
        return type switch
        {
            GachaConfigType.AvatarEventWish => 0,
            GachaConfigType.AvatarEventWish2 => 1,
            GachaConfigType.WeaponEventWish => 2,
            GachaConfigType.PermanentWish => 3,
            GachaConfigType.NoviceWish => 4,
            _ => 0,
        };
    }
}