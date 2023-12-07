// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 技能组
/// </summary>
[HighQuality]
internal sealed class SkillDepot
{
    private List<ProudableSkill>? compositeSkills;
    private List<ProudableSkill>? compositeSkillsNoInherents;

    /// <summary>
    /// 始基力
    /// </summary>
    public Arkhe Arkhe { get; set; }

    /// <summary>
    /// 技能天赋
    /// </summary>
    public List<ProudableSkill> Skills { get; set; } = default!;

    /// <summary>
    /// 大招
    /// </summary>
    public ProudableSkill EnergySkill { get; set; } = default!;

    /// <summary>
    /// 固有天赋
    /// </summary>
    public List<ProudableSkill> Inherents { get; set; } = default!;

    /// <summary>
    /// 全部天赋，包括固有天赋
    /// 在 Wiki 中使用
    /// </summary>
    public List<ProudableSkill> CompositeSkills { get => compositeSkills ??= [.. Skills, EnergySkill, .. Inherents]; }

    /// <summary>
    /// 命之座
    /// </summary>
    public List<Skill> Talents { get; set; } = default!;

    /// <summary>
    /// 获取无固有天赋的技能列表
    /// </summary>
    /// <returns>天赋列表</returns>
    public List<ProudableSkill> CompositeSkillsNoInherents()
    {
        // No Inherents                                  跳过 [替换冲刺的技能]
        return compositeSkillsNoInherents ??= [.. Skills.Where(s => s.Proud.Parameters.Count > 1), EnergySkill];
    }
}