// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class SkillDepot
{
    public Arkhe Arkhe { get; set; }

    public List<ProudableSkill> Skills { get; set; } = default!;

    public ProudableSkill EnergySkill { get; set; } = default!;

    public List<ProudableSkill> Inherents { get; set; } = default!;

    public List<Skill> Talents { get; set; } = default!;

    [field:MaybeNull]
    public List<ProudableSkill> CompositeSkills { get => field ??= [.. Skills, EnergySkill, .. Inherents]; }

    // No Inherents && 跳过 替换冲刺的技能
    [field: MaybeNull]
    public List<ProudableSkill> CompositeSkillsNoInherents { get => field ??= [.. Skills.Where(s => s.Proud.Parameters.Count > 1), EnergySkill]; }
}