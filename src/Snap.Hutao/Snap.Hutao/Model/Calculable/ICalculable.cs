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
    QualityType Quality { get; }

    /// <summary>
    /// 当前等级
    /// </summary>
    uint LevelCurrent { get; set; }

    /// <summary>
    /// 目标等级
    /// </summary>
    uint LevelTarget { get; set; }
}