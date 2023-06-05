// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 圣遗物
/// </summary>
[HighQuality]
internal sealed class Reliquary
{
    /// <summary>
    /// 等级 +20 = 21
    /// [1-21]
    /// Artifact Level [1-21]
    /// </summary>
    [JsonPropertyName("level")]
    public ReliquaryLevel Level { get; set; }

    /// <summary>
    /// 主属性Id
    /// Artifact Main Stat ID
    /// </summary>
    [JsonPropertyName("mainPropId")]
    public ReliquaryMainAffixId MainPropId { get; set; }

    /// <summary>
    /// 强化属性Id
    /// </summary>
    [JsonPropertyName("appendPropIdList")]
    public List<ReliquarySubAffixId> AppendPropIdList { get; set; } = default!;
}