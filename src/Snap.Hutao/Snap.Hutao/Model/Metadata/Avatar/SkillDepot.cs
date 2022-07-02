// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 技能组
/// </summary>
public class SkillDepot
{
    /// <summary>
    /// 技能天赋
    /// </summary>
    public IEnumerable<ProudableSkill> Skills { get; set; } = default!;

    /// <summary>
    /// 大招
    /// </summary>
    public ProudableSkill EnergySkill { get; set; } = default!;

    /// <summary>
    /// 固有天赋
    /// </summary>
    public IEnumerable<ProudableSkill> Inherents { get; set; } = default!;

    /// <summary>
    /// 命之座
    /// </summary>
    public IEnumerable<SkillBase> Talents { get; set; } = default!;
}
