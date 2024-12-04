// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

internal sealed class AvatarDetail
{
    [JsonPropertyName("skill_list")]
    public List<Skill> SkillList { get; set; } = default!;

    [JsonPropertyName("weapon")]
    public Weapon Weapon { get; set; } = default!;

    [JsonPropertyName("reliquary_list")]
    public List<Reliquary> ReliquaryList { get; set; } = default!;
}