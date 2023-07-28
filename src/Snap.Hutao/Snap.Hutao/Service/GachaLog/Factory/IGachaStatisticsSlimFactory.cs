// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.GachaLog;

namespace Snap.Hutao.Service.GachaLog.Factory;

/// <summary>
/// 简化的祈愿统计工厂
/// </summary>
internal interface IGachaStatisticsSlimFactory
{
    /// <summary>
    /// 异步创建一个新的简化的祈愿统计
    /// </summary>
    /// <param name="context">祈愿记录服务上下文</param>
    /// <param name="items">排序的物品</param>
    /// <param name="uid">uid</param>
    /// <returns>简化的祈愿统计</returns>
    ValueTask<GachaStatisticsSlim> CreateAsync(GachaLogServiceMetadataContext context, List<GachaItem> items, string uid);
}