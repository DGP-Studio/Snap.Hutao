// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

public sealed class ExclusiveTokenProvider : IDisposable
{
    private readonly Lock syncRoot = new();
    private CancellationTokenSource cts = new();

    public CancellationToken CurrentToken
    {
        get
        {
            lock (syncRoot)
            {
                return cts.Token;
            }
        }
    }

    public CancellationToken GetNewToken()
    {
        lock (syncRoot)
        {
            cts.Cancel();
            cts.Dispose();
            cts = new();
            return cts.Token;
        }
    }

    public void Cancel()
    {
        lock (syncRoot)
        {
            cts.Cancel();
        }
    }

    public void Dispose()
    {
        lock (syncRoot)
        {
            cts.Cancel();
            cts.Dispose();
        }
    }
}