// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.GachaLog;

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
    /// <param name="context">祈愿记录上下文</param>
    /// <returns>祈愿统计对象</returns>
    ValueTask<GachaStatistics> CreateAsync(IOrderedQueryable<GachaItem> items, GachaLogServiceContext context);
}