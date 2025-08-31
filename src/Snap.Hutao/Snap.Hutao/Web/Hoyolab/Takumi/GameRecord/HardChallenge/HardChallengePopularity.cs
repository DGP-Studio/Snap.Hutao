// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

internal sealed class HardChallengePopularity
{
    [JsonPropertyName("avatar_list")]
    public required ImmutableArray<HardChallengeSimpleAvatar> AvatarList { get; init; }
}