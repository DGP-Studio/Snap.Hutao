// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata;

/// <summary>
/// 生长曲线
/// </summary>
[HighQuality]
internal sealed class GrowCurve
{
    /// <summary>
    /// 等级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 曲线 值相乘
    /// </summary>
    public Dictionary<GrowCurveType, float> Curves { get; set; } = default!;
}