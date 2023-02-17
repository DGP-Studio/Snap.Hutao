// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 角色详情
/// </summary>
[HighQuality]
internal sealed class AvatarDetail
{
    /// <summary>
    /// 技能列表
    /// </summary>
    [JsonPropertyName("skill_list")]
    public List<Skill> SkillList { get; set; } = default!;

    /// <summary>
    /// 武器
    /// </summary>
    [JsonPropertyName("weapon")]
    public Weapon Weapon { get; set; } = default!;

    /// <summary>
    /// 圣遗物列表
    /// </summary>
    [JsonPropertyName("reliquary_list")]
    public List<Reliquary> ReliquaryList { get; set; } = default!;
}