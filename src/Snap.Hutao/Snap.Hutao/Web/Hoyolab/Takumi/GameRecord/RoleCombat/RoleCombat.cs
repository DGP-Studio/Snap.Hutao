// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombat
{
    [JsonPropertyName("data")]
    public List<RoleCombatData> Data { get; set; } = default!;

    [JsonPropertyName("is_unlock")]
    public bool IsUnlock { get; set; }

    [JsonPropertyName("links")]
    public RoleCombatLinks Links { get; set; } = default!;
}