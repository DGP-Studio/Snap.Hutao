// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-5-asyncsemaphore/
[SuppressMessage("", "SH003")]
internal sealed class AsyncSemaphore
{
    private readonly Queue<TaskCompletionSource> waiters = [];
    private readonly int maxCount;
    private int currentCount;

    public AsyncSemaphore(int initialCount, int maxCount = int.MaxValue)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(initialCount);
        currentCount = initialCount;
        this.maxCount = maxCount;
    }

    public Task WaitAsync()
    {
        lock (waiters)
        {
            if (currentCount > 0)
            {
                --currentCount;
                return Task.CompletedTask;
            }

            TaskCompletionSource waiter = new();
            waiters.Enqueue(waiter);
            return waiter.Task;
        }
    }

    public bool TryWait()
    {
        lock (waiters)
        {
            if (currentCount > 0)
            {
                --currentCount;
                return true;
            }

            return false;
        }
    }

    public int Release()
    {
        TaskCompletionSource? toRelease = default;
        int currentCountForReturn;
        lock (waiters)
        {
            if (waiters.Count > 0)
            {
                toRelease = waiters.Dequeue();
            }
            else
            {
                currentCount = Math.Min(currentCount + 1, maxCount);
            }

            currentCountForReturn = currentCount;
        }

        toRelease?.SetResult();
        return currentCountForReturn;
    }
}