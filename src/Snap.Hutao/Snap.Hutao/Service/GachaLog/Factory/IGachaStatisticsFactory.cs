// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Gacha;
using Snap.Hutao.Model.Entity;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 祈愿统计工厂
/// </summary>
[HighQuality]
internal interface IGachaStatisticsFactory
{
    /// <summary>
    /// 异步创建祈愿统计对象
    /// </summary>
    /// <param name="items">物品列表</param>
    /// <returns>祈愿统计对象</returns>
    Task<GachaStatistics> CreateAsync(IEnumerable<GachaItem> items);
}