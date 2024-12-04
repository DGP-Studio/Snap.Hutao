// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.GachaLog;

/// <summary>
/// 祈愿活动统计
/// </summary>
internal sealed class GachaEventStatistics
{
    /// <summary>
    /// 角色活动1
    /// </summary>
    public List<ItemCount> AvatarEvent { get; set; } = default!;

    /// <summary>
    /// 角色活动2
    /// </summary>
    public List<ItemCount> AvatarEvent2 { get; set; } = default!;

    /// <summary>
    /// 武器活动
    /// </summary>
    public List<ItemCount> WeaponEvent { get; set; } = default!;

    /// <summary>
    /// 集录祈愿
    /// </summary>
    public List<ItemCount> Chronicled { get; set; } = default!;
}