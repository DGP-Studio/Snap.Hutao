// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// 玩家统计数据
/// </summary>
[HighQuality]
internal sealed class PlayerStats
{
    /// <summary>
    /// 活跃天数
    /// </summary>
    [JsonPropertyName("active_day_number")]
    public int ActiveDayNumber { get; set; }

    /// <summary>
    /// 成就完成数
    /// </summary>
    [JsonPropertyName("achievement_number")]
    public int AchievementNumber { get; set; }

    /// <summary>
    /// 风神瞳个数
    /// </summary>
    [JsonPropertyName("anemoculus_number")]
    public int AnemoculusNumber { get; set; }

    /// <summary>
    /// 岩神瞳个数
    /// </summary>
    [JsonPropertyName("geoculus_number")]
    public int GeoculusNumber { get; set; }

    /// <summary>
    /// 雷神瞳个数
    /// </summary>
    [JsonPropertyName("electroculus_number")]
    public int ElectroculusNumber { get; set; }

    /// <summary>
    /// 草神瞳个数
    /// </summary>
    [JsonPropertyName("dendroculus_number")]
    public int DendroculusNumber { get; set; }

    /// <summary>
    /// 角色个数
    /// </summary>
    [JsonPropertyName("avatar_number")]
    public int AvatarNumber { get; set; }

    /// <summary>
    /// 传送锚点个数
    /// </summary>
    [JsonPropertyName("way_point_number")]
    public int WayPointNumber { get; set; }

    /// <summary>
    /// 秘境个数
    /// </summary>
    [JsonPropertyName("domain_number")]
    public int DomainNumber { get; set; }

    /// <summary>
    /// 深渊层数
    /// </summary>
    [JsonPropertyName("spiral_abyss")]
    public string SpiralAbyss { get; set; } = default!;

    /// <summary>
    /// 华丽宝箱个数
    /// </summary>
    [JsonPropertyName("luxurious_chest_number")]
    public int LuxuriousChestNumber { get; set; }

    /// <summary>
    /// 珍贵宝箱个数
    /// </summary>
    [JsonPropertyName("precious_chest_number")]
    public int PreciousChestNumber { get; set; }

    /// <summary>
    /// 精致宝箱个数
    /// </summary>
    [JsonPropertyName("exquisite_chest_number")]
    public int ExquisiteChestNumber { get; set; }

    /// <summary>
    /// 普通宝箱个数
    /// </summary>
    [JsonPropertyName("common_chest_number")]
    public int CommonChestNumber { get; set; }

    /// <summary>
    /// 奇馈宝箱
    /// </summary>
    [JsonPropertyName("magic_chest_number")]
    public int MagicChestNumber { get; set; }

    /// <summary>
    /// 额外的新数据
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}
