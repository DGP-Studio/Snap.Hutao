// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Annotation;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Enka.Model;

internal class ReliquarySubstat : Stat
{
    [JsonPropertyName("appendPropId")]
    [JsonEnum(JsonEnumSerializeType.String)]
    public FightProperty AppendPropId { get; set; }
}