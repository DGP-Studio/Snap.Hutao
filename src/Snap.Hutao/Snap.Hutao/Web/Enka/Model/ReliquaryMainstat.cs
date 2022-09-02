// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 圣遗物主属性
/// </summary>
public class ReliquaryMainstat
{
    /// <summary>
    /// Equipment Append Property Name.
    /// </summary>
    [JsonPropertyName("mainPropId")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FightProperty MainPropId { get; set; }

    /// <summary>
    /// Property Value
    /// </summary>
    [JsonPropertyName("statValue")]
    public double StatValue { get; set; }
}