// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

[HighQuality]
[SuppressMessage("", "CA1001")]
internal class ConcurrentCancellationTokenSource
{
    private CancellationTokenSource source = new();

    public CancellationToken Register()
    {
        source.Cancel();
        source.Dispose();

        source = new();
        return source.Token;
    }
}