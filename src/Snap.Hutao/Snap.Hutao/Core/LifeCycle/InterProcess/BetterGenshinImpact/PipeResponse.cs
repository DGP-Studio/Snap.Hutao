// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

internal class PipeResponse
{
    public required PipeResponseKind Kind { get; set; }
}

internal sealed class PipeResponse<T> : PipeResponse
{
    public T? Data { get; set; }
}