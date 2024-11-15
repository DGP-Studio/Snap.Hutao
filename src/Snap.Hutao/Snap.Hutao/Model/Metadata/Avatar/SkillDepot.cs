// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Avatar;

internal sealed class SkillDepot
{
    public required Arkhe Arkhe { get; init; }

    public required ImmutableArray<ProudableSkill> Skills { get; init; }

    public required ProudableSkill EnergySkill { get; init; }

    public required ImmutableArray<ProudableSkill> Inherents { get; init; }

    public required ImmutableArray<Skill> Talents { get; init; }

    [field: MaybeNull]
    public ImmutableArray<ProudableSkill> CompositeSkills { get => !field.IsDefault ? field : field = [.. Skills, EnergySkill, .. Inherents]; }

    // No Inherents && 跳过 替换冲刺的技能
    [field: MaybeNull]
    public ImmutableArray<ProudableSkill> CompositeSkillsNoInherents { get => !field.IsDefault ? field : field = [.. Skills.Where(s => s.Proud.Parameters.Count > 1), EnergySkill]; }
}