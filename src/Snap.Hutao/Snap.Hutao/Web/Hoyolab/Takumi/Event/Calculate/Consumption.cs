// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

internal class Consumption
{
    [JsonPropertyName("avatar_consume")]
    public List<Item>? AvatarConsume { get; set; }

    [JsonPropertyName("avatar_skill_consume")]
    public List<Item>? AvatarSkillConsume { get; set; }

    [JsonPropertyName("weapon_consume")]
    public List<Item>? WeaponConsume { get; set; }

    [JsonPropertyName("skills_consume")]
    public List<BatchSkillCosumption>? SkillsComsume { get; set; }
}