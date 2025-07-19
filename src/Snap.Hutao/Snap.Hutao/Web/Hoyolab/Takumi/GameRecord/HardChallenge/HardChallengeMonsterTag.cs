// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

internal sealed class HardChallengeMonsterTag
{
    [JsonPropertyName("type")]
    public required int Type { get; init; }

    [JsonPropertyName("desc")]
    public required string Description { get; init; }
}