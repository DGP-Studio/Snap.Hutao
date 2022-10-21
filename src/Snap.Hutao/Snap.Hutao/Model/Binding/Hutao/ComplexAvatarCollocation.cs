// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Model.Metadata.Converter;

namespace Snap.Hutao.Model.Binding.Hutao;

/// <summary>
/// 角色搭配
/// </summary>
internal class ComplexAvatarCollocation : ComplexAvatar
{
    /// <summary>
    /// 构造一个新的角色搭配
    /// </summary>
    /// <param name="avatar">角色</param>
    /// <param name="rate">比率</param>
    public ComplexAvatarCollocation(Avatar avatar)
        : base(avatar, 0)
    {
    }

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