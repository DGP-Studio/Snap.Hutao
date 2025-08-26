// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

internal sealed class HardChallengeLinks
{
    [JsonPropertyName("lineup_link")]
    public required Uri LineupLink { get; init; }

    [JsonPropertyName("play_link")]
    public required Uri PlayLink { get; init; }
}