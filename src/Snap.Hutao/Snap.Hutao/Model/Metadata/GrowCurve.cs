// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata;

/// <summary>
/// 生长曲线
/// </summary>
[HighQuality]
internal sealed class GrowCurve
{
    private Dictionary<GrowCurveType, float>? curveMap;

    /// <summary>
    /// 等级
    /// </summary>
    public Level Level { get; set; }

    /// <summary>
    /// 曲线 值相乘
    /// </summary>
    public List<TypeValue<GrowCurveType, float>> Curves { get; set; } = default!;

    /// <summary>
    /// 曲线映射
    /// </summary>
    public Dictionary<GrowCurveType, float> CurveMap
    {
        get => curveMap ??= Curves.ToDictionary(v => v.Type, v => v.Value);
    }
}