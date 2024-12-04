// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

internal sealed class ReliquarySet
{
    [JsonPropertyName("id")]
    public ReliquarySetId Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("affixes")]
    public List<ReliquaryAffix> Affixes { get; set; } = default!;
}