// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 胡桃云祈愿统计
/// </summary>
internal sealed class HutaoStatistics
{
    /// <summary>
    /// 角色活动
    /// </summary>
    public HutaoWishSummary AvatarEvent { get; set; } = default!;

    /// <summary>
    /// 角色活动2
    /// </summary>
    public HutaoWishSummary AvatarEvent2 { get; set; } = default!;

    /// <summary>
    /// 神铸赋形
    /// </summary>
    public HutaoWishSummary WeaponEvent { get; set; } = default!;

    /// <summary>
    /// 集录祈愿
    /// </summary>
    public HutaoWishSummary? Chronicled { get; set; }
}