// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Weapon;

/// <summary>
/// 生长曲线与值
/// </summary>
[HighQuality]
internal sealed class GrowCurveTypeValue
{
    /// <summary>
    /// 类型
    /// </summary>
    public GrowCurveType Type { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public float Value { get; set; }
}