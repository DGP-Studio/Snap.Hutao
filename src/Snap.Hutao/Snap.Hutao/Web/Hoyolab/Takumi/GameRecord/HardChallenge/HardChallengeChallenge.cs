// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

internal sealed class HardChallengeChallenge
{
    /// <summary>
    /// 怪物名称
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("second")]
    public required int Seconds { get; init; }

    [JsonPropertyName("teams")]
    public required ImmutableArray<HardChallengeAvatar> Team { get; init; }

    [JsonPropertyName("best_avatar")]
    public required ImmutableArray<HardChallengeBestAvatar> BestAvatars { get; init; }

    [JsonPropertyName("monster")]
    public required HardChallengeMonster Monster { get; init; }
}