// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatSplendourBuffLink
{
    [JsonPropertyName("id")]
    public required uint Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("desc")]
    public required string Description { get; init; }
}