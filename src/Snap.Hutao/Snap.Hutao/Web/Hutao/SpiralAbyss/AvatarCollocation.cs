// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

internal sealed class AvatarCollocation : AvatarBuild
{
    public List<ItemRate<AvatarId, double>> Avatars { get; set; } = default!;

    public List<ItemRate<WeaponId, double>> Weapons { get; set; } = default!;

    public List<ItemRate<ReliquarySets, double>> Reliquaries { get; set; } = default!;
}