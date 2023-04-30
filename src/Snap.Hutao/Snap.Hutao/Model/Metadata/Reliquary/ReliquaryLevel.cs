// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Reliquary;

/// <summary>
/// 圣遗物等级
/// </summary>
[HighQuality]
internal sealed class ReliquaryLevel
{
    /// <summary>
    /// 品质
    /// </summary>
    public ItemQuality Quality { get; set; }

    /// <summary>
    /// 等级 1-21
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 属性
    /// </summary>
    public Dictionary<FightProperty, float> Properties { get; set; } = default!;
}