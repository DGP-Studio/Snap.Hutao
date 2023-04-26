// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;

namespace Snap.Hutao.ViewModel.GachaLog;

/// <summary>
/// 历史物品
/// </summary>
[HighQuality]
internal sealed class StatisticsItem : Item
{
    /// <summary>
    /// 获取物品的个数
    /// </summary>
    public int Count { get; set; }
}