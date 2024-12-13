// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class SkillDepot
{
    public required Arkhe Arkhe { get; init; }

    public required ImmutableArray<ProudSkill> Skills { get; init; }

    public required ProudSkill EnergySkill { get; init; }

    public required ImmutableArray<ProudSkill> Inherents { get; init; }

    public required ImmutableArray<Skill> Talents { get; init; }

    public ImmutableArray<ProudSkill> CompositeSkills { get => !field.IsDefault ? field : field = [.. Skills, EnergySkill, .. Inherents]; }

    // No Inherents && 跳过 替换冲刺的技能
    public ImmutableArray<ProudSkill> CompositeSkillsNoInherents { get => !field.IsDefault ? field : field = [.. Skills.Where(s => s.Proud.Parameters.Count > 1), EnergySkill]; }
}