// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-3-asynccountdownevent/
[SuppressMessage("", "SH003")]
internal sealed class AsyncCountdownEvent
{
    private readonly AsyncManualResetEvent amre = new();
    private int count;

    public AsyncCountdownEvent(int initialCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(initialCount);
        count = initialCount;
    }

    public Task WaitAsync()
    {
        return amre.WaitAsync();
    }

    public void Signal()
    {
        if (count <= 0)
        {
            throw new InvalidOperationException();
        }

        int newCount = Interlocked.Decrement(ref count);
        if (newCount == 0)
        {
            amre.Set();
        }
        else if (newCount < 0)
        {
            throw new InvalidOperationException();
        }
    }

    public Task SignalAndWait()
    {
        Signal();
        return WaitAsync();
    }
}