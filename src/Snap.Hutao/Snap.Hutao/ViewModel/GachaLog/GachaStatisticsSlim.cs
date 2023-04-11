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
    public TypedWishSummary AvatarWish { get; set; } = default!;

    /// <summary>
    /// 神铸赋形
    /// </summary>
    public TypedWishSummary WeaponWish { get; set; } = default!;

    /// <summary>
    /// 奔行世间
    /// </summary>
    public TypedWishSummary StandardWish { get; set; } = default!;
}