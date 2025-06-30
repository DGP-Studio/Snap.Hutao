// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatBuff
{
    [JsonPropertyName("icon")]
    public required string Icon { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("desc")]
    public required string Description { get; init; }

    [JsonPropertyName("is_enhanced")]
    public required bool IsEnhanced { get; init; }

    [JsonPropertyName("id")]
    public required uint Id { get; init; }
}