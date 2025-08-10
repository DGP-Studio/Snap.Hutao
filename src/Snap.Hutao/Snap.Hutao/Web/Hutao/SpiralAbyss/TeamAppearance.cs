// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

internal sealed class TeamAppearance
{
    public int Floor { get; set; }

    public ImmutableArray<ItemRate<string, int>> Up { get; set; }

    public ImmutableArray<ItemRate<string, int>> Down { get; set; }
}