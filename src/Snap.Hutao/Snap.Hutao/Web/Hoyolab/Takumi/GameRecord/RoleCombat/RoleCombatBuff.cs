// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatBuff
{
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("desc")]
    public string Description { get; set; } = default!;

    [JsonPropertyName("is_enhanced")]
    public bool IsEnhanced { get; set; }

    [JsonPropertyName("id")]
    public uint Id { get; set; }
}