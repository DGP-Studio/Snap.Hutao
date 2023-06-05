// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Annotation;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 圣遗物副属性
/// </summary>
[HighQuality]
internal class ReliquarySubstat : Stat
{
    /// <summary>
    /// 增加属性
    /// </summary>
    [JsonPropertyName("appendPropId")]
    [JsonEnum(JsonSerializeType.String)]
    public FightProperty AppendPropId { get; set; }
}