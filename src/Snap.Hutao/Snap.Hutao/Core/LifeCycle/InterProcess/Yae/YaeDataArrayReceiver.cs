// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using System.Collections.Immutable;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.Yae;

internal sealed class YaeDataArrayReceiver : IDisposable
{
    public ImmutableArray<YaeData> Array { get; set; }

    public void Dispose()
    {
        foreach (ref readonly YaeData data in Array.AsSpan())
        {
            data.Dispose();
        }
    }
}