// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

internal sealed class HardChallengeData
{
    [JsonPropertyName("schedule")]
    public required HardChallengeSchedule Schedule { get; init; }

    [JsonPropertyName("single")]
    public required HardChallengeDataEntry SinglePlayer { get; init; }

    [JsonPropertyName("mp")]
    public required HardChallengeDataEntry MultiPlayer { get; init; }

    [JsonPropertyName("blings")]
    public required ImmutableArray<HardChallengeBlingAvatar> Blings { get; init; }
}