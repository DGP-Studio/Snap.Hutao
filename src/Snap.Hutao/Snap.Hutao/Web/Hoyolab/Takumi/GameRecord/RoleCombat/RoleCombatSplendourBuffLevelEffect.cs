// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatSplendourBuffLevelEffect
{
    [JsonPropertyName("icon")]
    public required string Icon { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("desc")]
    public required string Description { get; init; }

    [JsonPropertyName("links")]
    public ImmutableDictionary<uint, RoleCombatSplendourBuffLink>? Links { get; init; }
}