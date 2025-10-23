// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game.Island;

internal sealed class IslandFeature2
{
    public required IslandFunctionOffsets Oversea { get; set; }

    public required IslandFunctionOffsets Chinese { get; set; }

    public string? Message { get; set; }
}