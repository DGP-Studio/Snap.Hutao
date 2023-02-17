// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 角色搭配
/// </summary>
[HighQuality]
internal sealed class AvatarCollocation : AvatarBuild
{
    /// <summary>
    /// 其他角色
    /// </summary>
    public List<ItemRate<int, double>> Avatars { get; set; } = default!;

    /// <summary>
    /// 武器
    /// </summary>
    public List<ItemRate<int, double>> Weapons { get; set; } = default!;

    /// <summary>
    /// 圣遗物
    /// </summary>
    public List<ItemRate<ReliquarySets, double>> Reliquaries { get; set; } = default!;
}