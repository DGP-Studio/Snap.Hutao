// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 武器属性
/// </summary>
public class WeaponStat
{
    /// <summary>
    /// 提升属性Id
    /// </summary>
    [JsonPropertyName("appendPropId")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FightProperty AppendPropId { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    [JsonPropertyName("statValue")]
    public double StatValue { get; set; }
}