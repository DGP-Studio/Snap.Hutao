// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

internal sealed class SpiralAbyssMonster
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("icon")]
    public required string Icon { get; init; }

    [JsonPropertyName("level")]
    public required int Level { get; init; }
}