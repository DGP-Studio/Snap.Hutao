// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata;

namespace Snap.Hutao.Model.Binding.AvatarProperty;

/// <summary>
/// 天赋
/// </summary>
public class Skill : NameIconDescription
{
    /// <summary>
    /// 技能属性
    /// </summary>
    public LevelParam<string, ParameterInfo> Info { get; set; } = default!;
}
