// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 简化的祈愿统计
/// </summary>
internal sealed class GachaStatisticsSlim
{
    /// <summary>
    /// Uid
    /// </summary>
    public string Uid { get; set; } = default!;

    /// <summary>
    /// 角色活动
    /// </summary>
    public TypedWishSummarySlim AvatarWish { get; set; } = default!;

    /// <summary>
    /// 神铸赋形
    /// </summary>
    public TypedWishSummarySlim WeaponWish { get; set; } = default!;

    /// <summary>
    /// 奔行世间
    /// </summary>
    public TypedWishSummarySlim StandardWish { get; set; } = default!;
}