// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 祈愿统计概览
/// </summary>
internal sealed class HutaoWishSummary
{
    /// <summary>
    /// 卡池
    /// </summary>
    public GachaEvent Event { get; set; } = default!;

    /// <summary>
    /// 五星物品
    /// </summary>
    public List<StatisticsItem> OrangeItems { get; set; } = default!;

    /// <summary>
    /// 四星物品
    /// </summary>
    public List<StatisticsItem> PurpleItems { get; set; } = default!;

    /// <summary>
    /// 三星物品
    /// </summary>
    public List<StatisticsItem> BlueItems { get; set; } = default!;
}