// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Reliquary;

/// <summary>
/// 圣遗物等级
/// </summary>
[HighQuality]
internal sealed partial class ReliquaryMainAffixLevel
{
    /// <summary>
    /// 品质
    /// </summary>
    public QualityType Rank { get; set; }

    /// <summary>
    /// 等级 1-21
    /// </summary>
    public uint Level { get; set; }

    /// <summary>
    /// 属性
    /// </summary>
    public List<TypeValue<FightProperty, float>> Properties { get; set; } = default!;
}