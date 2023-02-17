// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 武器搭配
/// </summary>
[HighQuality]
internal sealed class WeaponCollocation : WeaponBuild
{
    /// <summary>
    /// 其他角色
    /// </summary>
    public List<ItemRate<int, double>> Avatars { get; set; } = default!;
}