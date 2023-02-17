// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Annotation;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 圣遗物主属性
/// </summary>
[HighQuality]
internal sealed class ReliquaryMainstat
{
    /// <summary>
    /// Equipment Append Property Name.
    /// </summary>
    [JsonPropertyName("mainPropId")]
    [JsonEnum(JsonSerializeType.String)]
    public FightProperty MainPropId { get; set; }

    /// <summary>
    /// Property Value
    /// </summary>
    [JsonPropertyName("statValue")]
    public double StatValue { get; set; }
}