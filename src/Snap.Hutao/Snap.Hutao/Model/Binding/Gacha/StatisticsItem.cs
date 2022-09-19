// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Gacha.Abstraction;

namespace Snap.Hutao.Model.Binding.Gacha;

/// <summary>
/// 历史物品
/// </summary>
public class StatisticsItem : ItemBase
{
    /// <summary>
    /// 获取物品的个数
    /// </summary>
    public int Count { get; set; }
}
