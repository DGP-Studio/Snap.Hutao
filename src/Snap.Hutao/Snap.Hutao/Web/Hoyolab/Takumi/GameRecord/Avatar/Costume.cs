// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

internal sealed class Costume
{
    [JsonPropertyName("id")]
    public CostumeId Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    // Ignored field string icon
}