﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Annotation;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Enka.Model;

/// <summary>
/// 圣遗物主属性
/// </summary>
[HighQuality]
internal sealed class ReliquaryMainstat : Stat
{
    /// <summary>
    /// Equipment Append Property Name.
    /// </summary>
    [JsonPropertyName("mainPropId")]
    [JsonEnum(JsonEnumSerializeType.String)]
    public FightProperty MainPropId { get; set; }
}