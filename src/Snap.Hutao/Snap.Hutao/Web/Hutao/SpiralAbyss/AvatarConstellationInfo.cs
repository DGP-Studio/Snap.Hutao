// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

internal sealed class AvatarConstellationInfo : AvatarBuild
{
    public double HoldingRate { get; set; }

    public List<ItemRate<int, double>> Constellations { get; set; } = default!;
}