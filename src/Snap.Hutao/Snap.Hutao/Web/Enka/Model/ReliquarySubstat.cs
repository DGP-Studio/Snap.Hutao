// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 圣遗物副属性
/// </summary>
public class ReliquarySubstat
{
    /// <summary>
    /// 增加属性
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