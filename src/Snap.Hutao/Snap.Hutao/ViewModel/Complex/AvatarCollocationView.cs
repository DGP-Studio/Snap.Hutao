// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.Complex;

/// <summary>
/// 角色搭配
/// </summary>
[HighQuality]
internal sealed class AvatarCollocationView
{
    /// <summary>
    /// 角色Id
    /// </summary>
    public AvatarId AvatarId { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    public List<AvatarView> Avatars { get; set; } = default!;

    /// <summary>
    /// 武器
    /// </summary>
    public List<WeaponView> Weapons { get; set; } = default!;

    /// <summary>
    /// 圣遗物套装
    /// </summary>
    public List<ReliquarySetView> ReliquarySets { get; set; } = default!;
}