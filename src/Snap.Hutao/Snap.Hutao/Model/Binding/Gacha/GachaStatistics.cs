// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding.Gacha;

/// <summary>
/// 祈愿统计
/// </summary>
public class GachaStatistics
{
    /// <summary>
    /// 默认的空祈愿统计
    /// </summary>
    public static readonly GachaStatistics Default = new();

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
    public TypedWishSummary PermanentWish { get; set; } = default!;

    /// <summary>
    /// 历史
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