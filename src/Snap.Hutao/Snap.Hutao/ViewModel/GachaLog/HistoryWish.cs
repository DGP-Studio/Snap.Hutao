// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 历史卡池概览
/// </summary>
[HighQuality]
internal sealed class HistoryWish : Wish
{
    /// <summary>
    /// 版本
    /// </summary>
    public string Version { get; set; } = default!;

    /// <summary>
    /// 卡池图片
    /// </summary>
    public Uri BannerImage { get; set; } = default!;

    /// <summary>
    /// 五星Up
    /// </summary>
    public List<StatisticsItem> OrangeUpList { get; set; } = default!;

    /// <summary>
    /// 四星Up
    /// </summary>
    public List<StatisticsItem> PurpleUpList { get; set; } = default!;

    /// <summary>
    /// 五星Up
    /// </summary>
    public List<StatisticsItem> OrangeList { get; set; } = default!;

    /// <summary>
    /// 四星Up
    /// </summary>
    public List<StatisticsItem> PurpleList { get; set; } = default!;

    /// <summary>
    /// 三星Up
    /// </summary>
    public List<StatisticsItem> BlueList { get; set; } = default!;
}