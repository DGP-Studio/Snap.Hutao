// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Binding.Hutao;

/// <summary>
/// 武器搭配
/// </summary>
public class ComplexWeaponCollocation
{
    /// <summary>
    /// 武器Id
    /// </summary>
    public WeaponId WeaponId { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    public List<ComplexAvatar> Avatars { get; set; } = default!;
}