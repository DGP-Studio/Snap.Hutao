// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

internal class PromotionDelta
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("level_current")]
    public uint LevelCurrent { get; set; }

    [JsonPropertyName("level_target")]
    public uint LevelTarget { get; set; }

    [JsonIgnore]
    public uint WeaponPromoteLevel { get; set; }
}