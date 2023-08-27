// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

/// <summary>
/// 角色搭配
/// </summary>
[HighQuality]
internal sealed class AvatarCollocation : AvatarBuild
{
    /// <summary>
    /// 其他角色
    /// </summary>
    public List<ItemRate<AvatarId, double>> Avatars { get; set; } = default!;

    /// <summary>
    /// 武器
    /// </summary>
    public List<ItemRate<WeaponId, double>> Weapons { get; set; } = default!;

    /// <summary>
    /// 圣遗物
    /// </summary>
    public List<ItemRate<ReliquarySets, double>> Reliquaries { get; set; } = default!;
}