// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Annotation;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 武器属性
/// </summary>
internal sealed class WeaponStat
{
    /// <summary>
    /// 提升属性Id
    /// </summary>
    [JsonPropertyName("appendPropId")]
    [JsonEnum(JsonSerializeType.String)]
    public FightProperty AppendPropId { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    [JsonPropertyName("statValue")]
    public float StatValue { get; set; }
}