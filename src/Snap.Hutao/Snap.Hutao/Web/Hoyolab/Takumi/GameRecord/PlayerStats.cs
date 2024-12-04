// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

internal sealed class PlayerStats
{
    [JsonPropertyName("active_day_number")]
    public int ActiveDayNumber { get; set; }

    [JsonPropertyName("achievement_number")]
    public int AchievementNumber { get; set; }

    [JsonPropertyName("anemoculus_number")]
    public int AnemoculusNumber { get; set; }

    [JsonPropertyName("geoculus_number")]
    public int GeoculusNumber { get; set; }

    [JsonPropertyName("electroculus_number")]
    public int ElectroculusNumber { get; set; }

    [JsonPropertyName("dendroculus_number")]
    public int DendroculusNumber { get; set; }

    [JsonPropertyName("avatar_number")]
    public int AvatarNumber { get; set; }

    [JsonPropertyName("way_point_number")]
    public int WayPointNumber { get; set; }

    [JsonPropertyName("domain_number")]
    public int DomainNumber { get; set; }

    [JsonPropertyName("spiral_abyss")]
    public string SpiralAbyss { get; set; } = default!;

    [JsonPropertyName("luxurious_chest_number")]
    public int LuxuriousChestNumber { get; set; }

    [JsonPropertyName("precious_chest_number")]
    public int PreciousChestNumber { get; set; }

    [JsonPropertyName("exquisite_chest_number")]
    public int ExquisiteChestNumber { get; set; }

    [JsonPropertyName("common_chest_number")]
    public int CommonChestNumber { get; set; }

    [JsonPropertyName("magic_chest_number")]
    public int MagicChestNumber { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
