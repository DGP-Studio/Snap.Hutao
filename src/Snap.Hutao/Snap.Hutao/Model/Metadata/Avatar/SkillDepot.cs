// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 技能组
/// </summary>
public class SkillDepot
{
    /// <summary>
    /// 技能天赋
    /// </summary>
    public IList<ProudableSkill> Skills { get; set; } = default!;

    /// <summary>
    /// 大招
    /// </summary>
    public ProudableSkill EnergySkill { get; set; } = default!;

    /// <summary>
    /// 固有天赋
    /// </summary>
    public IList<ProudableSkill> Inherents { get; set; } = default!;

    /// <summary>
    /// 全部天赋,包括固有天赋
    /// </summary>
    public IList<ProudableSkill> CompositeSkills
    {
        get => GetCompositeSkills().ToList();
    }

    /// <summary>
    /// 命之座
    /// </summary>
    public IList<SkillBase> Talents { get; set; } = default!;

    /// <summary>
    /// 获取无固有天赋的技能列表
    /// </summary>
    /// <returns>天赋列表</returns>
    public IEnumerable<ProudableSkill> GetCompositeSkillsNoInherents()
    {
        foreach (ProudableSkill skill in Skills)
        {
            // skip skills like Mona's & Ayaka's shift
            if (skill.Proud.Parameters.Count > 1)
            {
                yield return skill;
            }
        }

        yield return EnergySkill;
    }

    private IEnumerable<ProudableSkill> GetCompositeSkills()
    {
        foreach (ProudableSkill skill in Skills)
        {
            yield return skill;
        }

        yield return EnergySkill;

        foreach (ProudableSkill skill in Inherents)
        {
            yield return skill;
        }
    }
}
