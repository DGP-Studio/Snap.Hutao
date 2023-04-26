// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 技能信息
/// </summary>
[HighQuality]
internal sealed partial class ProudableSkill : Skill
{
    /// <summary>
    /// 组Id
    /// </summary>
    public SkillGroupId GroupId { get; set; }

    /// <summary>
    /// 提升属性
    /// </summary>
    public DescriptionsParameters Proud { get; set; } = default!;
}