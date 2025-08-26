// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

internal sealed class HardChallengeMonster
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("level")]
    public required uint Level { get; init; }

    [JsonPropertyName("icon")]
    public required Uri Icon { get; init; }

    [JsonPropertyName("desc")]
    public required ImmutableArray<string> Descriptions { get; set; }

    [JsonPropertyName("tags")]
    public required ImmutableArray<HardChallengeMonsterTag> Tags { get; init; }

    [JsonPropertyName("monster_id")]
    public required MonsterId MonsterId { get; init; }
}