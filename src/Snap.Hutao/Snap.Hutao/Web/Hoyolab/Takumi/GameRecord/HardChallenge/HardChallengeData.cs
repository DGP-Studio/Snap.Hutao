// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

internal sealed class HardChallengeData
{
    [JsonPropertyName("schedule")]
    public required HardChallengeSchedule Schedule { get; init; }

    [JsonPropertyName("single")]
    public required HardChallengeEntry SinglePlayer { get; init; }

    [JsonPropertyName("mp")]
    public required HardChallengeEntry MultiPlayer { get; init; }

    [JsonPropertyName("blings")]
    public required JsonElement Blings { get; init; }
}