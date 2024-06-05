// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 消耗
/// </summary>
[HighQuality]
internal class Consumption
{
    /// <summary>
    /// 角色等级消耗
    /// </summary>
    [JsonPropertyName("avatar_consume")]
    public List<Item>? AvatarConsume { get; set; }

    /// <summary>
    /// 技能消耗
    /// </summary>
    [JsonPropertyName("avatar_skill_consume")]
    public List<Item>? AvatarSkillConsume { get; set; }

    /// <summary>
    /// 武器消耗
    /// </summary>
    [JsonPropertyName("weapon_consume")]
    public List<Item>? WeaponConsume { get; set; }

    [JsonPropertyName("skills_consume")]
    public List<BatchSkillCosumption>? SkillsComsume { get; set; }
}