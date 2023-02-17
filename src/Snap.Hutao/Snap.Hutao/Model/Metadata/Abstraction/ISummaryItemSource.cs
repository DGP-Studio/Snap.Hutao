// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Gacha;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Abstraction;

/// <summary>
/// 指示该类为简述统计物品的源
/// </summary>
[HighQuality]
internal interface ISummaryItemSource
{
    /// <summary>
    /// 星级
    /// </summary>
    ItemQuality Quality { get; }

    /// <summary>
    /// 转换到简述统计物品
    /// </summary>
    /// <param name="lastPull">距上个五星</param>
    /// <param name="time">时间</param>
    /// <param name="isUp">是否为Up物品</param>
    /// <returns>简述统计物品</returns>
    SummaryItem ToSummaryItem(int lastPull, DateTimeOffset time, bool isUp);
}