// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

internal sealed partial class ExclusiveTokenProvider : IDisposable
{
    private readonly Lock syncRoot = new();
    private volatile CancellationTokenSource? cts = new();

    public CancellationToken CurrentToken
    {
        get
        {
            lock (syncRoot)
            {
                return cts?.Token ?? new(true);
            }
        }
    }

    public CancellationToken GetNewToken()
    {
        lock (syncRoot)
        {
            if (cts is not null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            cts = new();
            return cts.Token;
        }
    }

    public void Cancel()
    {
        lock (syncRoot)
        {
            cts?.Cancel();
        }
    }

    public void Dispose()
    {
        lock (syncRoot)
        {
            if (cts is not null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            cts = default!;
        }
    }
}