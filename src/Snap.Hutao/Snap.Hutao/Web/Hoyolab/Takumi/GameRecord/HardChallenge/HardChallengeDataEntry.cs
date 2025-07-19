// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

internal sealed class HardChallengeDataEntry
{
    [JsonPropertyName("best")]
    public HardChallengeBest? Best { get; init; }

    [JsonPropertyName("challenge")]
    public required ImmutableArray<HardChallengeChallenge> Challenges { get; init; }

    [JsonPropertyName("has_data")]
    [MemberNotNull(nameof(Best))]
    public required bool HasData { get; init; }
}