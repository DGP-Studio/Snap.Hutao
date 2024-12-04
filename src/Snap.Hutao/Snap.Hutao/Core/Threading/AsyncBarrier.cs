// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Concurrent;

namespace Snap.Hutao.Core.Threading;

// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-4-asyncbarrier/
[SuppressMessage("", "SH003")]
internal sealed class AsyncBarrier
{
    private readonly int participantCount;
    private int remainingParticipants;
    private ConcurrentStack<TaskCompletionSource> waiters = [];

    public AsyncBarrier(int participantCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(participantCount);
        remainingParticipants = this.participantCount = participantCount;
    }

    public Task SignalAndWaitAsync()
    {
        TaskCompletionSource tcs = new();
        waiters.Push(tcs);
        if (Interlocked.Decrement(ref remainingParticipants) == 0)
        {
            remainingParticipants = participantCount;
            ConcurrentStack<TaskCompletionSource> waiters = this.waiters;
            this.waiters = [];
            Parallel.ForEach(waiters, w => w.SetResult());
        }

        return tcs.Task;
    }
}