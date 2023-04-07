// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录
/// </summary>
internal static class GachaLog
{
    /// <summary>
    /// 查询类型
    /// </summary>
    public static readonly ImmutableList<GachaConfigType> QueryTypes = new List<GachaConfigType>
    {
        GachaConfigType.NoviceWish,
        GachaConfigType.StandardWish,
        GachaConfigType.AvatarEventWish,
        GachaConfigType.WeaponEventWish,
    }.ToImmutableList();
}