// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Annotation;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Enka.Model;

internal sealed class ReliquaryMainstat : Stat
{
    [JsonPropertyName("mainPropId")]
    [JsonEnum(JsonEnumSerializeType.String)]
    public FightProperty MainPropId { get; set; }
}