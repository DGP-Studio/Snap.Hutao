// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

internal sealed class AvatarAppearanceRank
{
    public int Floor { get; set; }

    public List<ItemRate<AvatarId, double>> Ranks { get; set; } = default!;
}