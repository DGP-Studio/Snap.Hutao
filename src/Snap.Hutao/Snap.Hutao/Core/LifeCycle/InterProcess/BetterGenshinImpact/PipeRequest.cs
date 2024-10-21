// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.IO.Pipes;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

internal sealed class PipeRequest<T>
{
    public required PipeRequestKind Kind { get; set; }

    public required T Data { get; set; }
}