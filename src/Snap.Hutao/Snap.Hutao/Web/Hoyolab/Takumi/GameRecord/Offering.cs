// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

internal sealed class Offering
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("level")]
    public int Level { get; set; } = default!;

    [JsonPropertyName("icon")]
    public Uri Icon { get; set; } = default!;
}