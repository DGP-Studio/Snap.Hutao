// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 技能信息
/// </summary>
public partial class ProudableSkill : SkillBase
{
    /// <summary>
    /// 组Id
    /// </summary>
    public int GroupId { get; set; }

    /// <summary>
    /// 提升属性
    /// </summary>
    public DescParam Proud { get; set; } = default!;
}