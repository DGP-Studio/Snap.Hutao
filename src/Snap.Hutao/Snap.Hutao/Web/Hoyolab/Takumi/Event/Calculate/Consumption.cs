// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

// Api will return empty array if no consumption
internal class Consumption
{
    [JsonPropertyName("avatar_consume")]
    public ImmutableArray<Item> AvatarConsume { get; set; }

    [JsonPropertyName("avatar_skill_consume")]
    public ImmutableArray<Item> AvatarSkillConsume { get; set; }

    [JsonPropertyName("weapon_consume")]
    public ImmutableArray<Item> WeaponConsume { get; set; }

    [JsonPropertyName("skills_consume")]
    public ImmutableArray<BatchSkillCosumption> SkillsConsume { get; set; }
}