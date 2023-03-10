// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 祈愿统计
/// </summary>
[HighQuality]
internal sealed class GachaStatistics
{
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

    /// <summary>
    /// 历史卡池
    /// </summary>
    public List<HistoryWish> HistoryWishes { get; set; } = default!;

    /// <summary>
    /// 五星角色
    /// </summary>
    public List<StatisticsItem> OrangeAvatars { get; set; } = default!;

    /// <summary>
    /// 四星角色
    /// </summary>
    public List<StatisticsItem> PurpleAvatars { get; set; } = default!;

    /// <summary>
    /// 五星武器
    /// </summary>
    public List<StatisticsItem> OrangeWeapons { get; set; } = default!;

    /// <summary>
    /// 四星武器
    /// </summary>
    public List<StatisticsItem> PurpleWeapons { get; set; } = default!;

    /// <summary>
    /// 三星武器
    /// </summary>
    public List<StatisticsItem> BlueWeapons { get; set; } = default!;
}