// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 技能基础
/// 命座
/// </summary>
[HighQuality]
internal class Skill
{
    /// <summary>
    /// Id
    /// </summary>
    public SkillId Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; } = default!;
}