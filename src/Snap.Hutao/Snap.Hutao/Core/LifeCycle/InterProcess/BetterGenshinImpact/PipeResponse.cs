// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

internal sealed class PipeResponse
{
    public required PipeResponseKind Kind { get; set; }

    public required JsonElement Data { get; set; }
}