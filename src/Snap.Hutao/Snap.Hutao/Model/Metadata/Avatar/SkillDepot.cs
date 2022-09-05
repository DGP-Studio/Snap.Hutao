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
    /// 全部天赋
    /// </summary>
    public IList<ProudableSkill> CompositeSkills
    {
        get => GetCompositeSkills().ToList();
    }

    /// <summary>
    /// 命之座
    /// </summary>
    public IList<SkillBase> Talents { get; set; } = default!;

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
