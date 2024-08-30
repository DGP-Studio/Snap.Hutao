// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class SkillDepot
{
    private List<ProudableSkill>? compositeSkills;
    private List<ProudableSkill>? compositeSkillsNoInherents;

    public Arkhe Arkhe { get; set; }

    public List<ProudableSkill> Skills { get; set; } = default!;

    public ProudableSkill EnergySkill { get; set; } = default!;

    public List<ProudableSkill> Inherents { get; set; } = default!;

    public List<ProudableSkill> CompositeSkills { get => compositeSkills ??= [.. Skills, EnergySkill, .. Inherents]; }

    public List<Skill> Talents { get; set; } = default!;

    public List<ProudableSkill> CompositeSkillsNoInherents()
    {
        // No Inherents && 跳过 替换冲刺的技能
        return compositeSkillsNoInherents ??= [.. Skills.Where(s => s.Proud.Parameters.Count > 1), EnergySkill];
    }
}