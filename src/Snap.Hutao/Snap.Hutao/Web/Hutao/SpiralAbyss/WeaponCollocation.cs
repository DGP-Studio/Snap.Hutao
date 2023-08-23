// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

/// <summary>
/// 武器搭配
/// </summary>
[HighQuality]
internal sealed class WeaponCollocation : WeaponBuild
{
    /// <summary>
    /// 其他角色
    /// </summary>
    public List<ItemRate<AvatarId, double>> Avatars { get; set; } = default!;
}