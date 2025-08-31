// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-3-asynccountdownevent/
[SuppressMessage("", "SH003")]
internal sealed class AsyncCountdownEvent
{
    private readonly AsyncManualResetEvent asyncManualResetEvent = new();
    private int count;

    public AsyncCountdownEvent(int initialCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(initialCount);
        count = initialCount;
    }

    public Task WaitAsync()
    {
        return asyncManualResetEvent.WaitAsync();
    }

    public void Signal()
    {
        Verify.Operation(count > 0, "Cannot signal when count is less than or equals to 0.");

        int newCount = Interlocked.Decrement(ref count);
        switch (newCount)
        {
            case 0:
                asyncManualResetEvent.Set();
                break;
            case < 0:
                throw new InvalidOperationException();
        }
    }
}