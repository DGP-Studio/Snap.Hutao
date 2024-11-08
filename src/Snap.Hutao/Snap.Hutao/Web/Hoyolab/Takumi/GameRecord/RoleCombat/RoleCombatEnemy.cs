// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatEnemy
{
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = default!;

    [JsonPropertyName("level")]
    public uint Level { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
}