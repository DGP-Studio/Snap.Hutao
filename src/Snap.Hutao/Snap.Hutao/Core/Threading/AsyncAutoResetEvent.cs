// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-2-asyncautoresetevent/
[SuppressMessage("", "SH003")]
internal sealed class AsyncAutoResetEvent
{
    private readonly Queue<TaskCompletionSource> waits = [];
    private bool signaled;

    public AsyncAutoResetEvent(bool initialState)
    {
        signaled = initialState;
    }

    public Task WaitOneAsync()
    {
        lock (waits)
        {
            if (signaled)
            {
                signaled = false;
                return Task.CompletedTask;
            }

            TaskCompletionSource tcs = new();
            waits.Enqueue(tcs);
            return tcs.Task;
        }
    }

    public void Set()
    {
        TaskCompletionSource? toRelease = default;
        lock (waits)
        {
            if (waits.Count > 0)
            {
                toRelease = waits.Dequeue();
            }
            else if (!signaled)
            {
                signaled = true;
            }
        }

        toRelease?.SetResult();
    }
}