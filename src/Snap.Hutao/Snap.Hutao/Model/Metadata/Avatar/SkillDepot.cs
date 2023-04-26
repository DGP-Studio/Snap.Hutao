// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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
    public List<ProudableSkill> CompositeSkills
    {
        get
        {
            if (compositeSkills == null)
            {
                compositeSkills = new(Skills.Count + 1 + Inherents.Count);
                compositeSkills.AddRange(Skills);
                compositeSkills.Add(EnergySkill);
                compositeSkills.AddRange(Inherents);
            }

            return compositeSkills;
        }
    }

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
        if (compositeSkillsNoInherents == null)
        {
            compositeSkillsNoInherents = new(Skills.Count + 1);
            compositeSkillsNoInherents.AddRange(Skills);
            compositeSkillsNoInherents.Add(EnergySkill);
        }

        // No Inherents
        return compositeSkillsNoInherents;
    }
}