// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Calculable;

/// <summary>
/// 可计算源
/// </summary>
[HighQuality]
internal interface ICalculable : INameIcon
{
    /// <summary>
    /// 星级
    /// </summary>
    ItemQuality Quality { get; }

    /// <summary>
    /// 当前等级
    /// </summary>
    int LevelCurrent { get; set; }

    /// <summary>
    /// 目标等级
    /// </summary>
    int LevelTarget { get; set; }
}