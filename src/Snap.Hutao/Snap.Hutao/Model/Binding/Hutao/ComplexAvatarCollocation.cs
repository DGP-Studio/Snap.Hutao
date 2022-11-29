// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Binding.Hutao;

/// <summary>
/// 角色搭配
/// </summary>
public class ComplexAvatarCollocation
{
    /// <summary>
    /// 角色Id
    /// </summary>
    public AvatarId AvatarId { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    public List<ComplexAvatar> Avatars { get; set; } = default!;

    /// <summary>
    /// 武器
    /// </summary>
    public List<ComplexWeapon> Weapons { get; set; } = default!;

    /// <summary>
    /// 圣遗物套装
    /// </summary>
    public List<ComplexReliquarySet> ReliquarySets { get; set; } = default!;
}