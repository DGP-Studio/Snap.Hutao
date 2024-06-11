// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.GachaLog;

namespace Snap.Hutao.Model.Metadata.Abstraction;

/// <summary>
/// 指示该类为统计物品的源
/// </summary>
[HighQuality]
internal interface IStatisticsItemConvertible
{
    /// <summary>
    /// 转换到统计物品
    /// </summary>
    /// <param name="count">个数</param>
    /// <returns>统计物品</returns>
    StatisticsItem ToStatisticsItem(int count);
}