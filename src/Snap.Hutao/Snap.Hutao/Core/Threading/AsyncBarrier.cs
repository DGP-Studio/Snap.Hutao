// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

internal class AsyncBarrier
{
    private readonly int participantCount;
    private readonly Queue<TaskCompletionSource> waiters;

    public AsyncBarrier(int participants)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(participants, "Participants of AsyncBarrier must be greater than 0");
        participantCount = participants;

        // Allocate the stack so no resizing is necessary.
        // We don't need space for the last participant, since we never have to store it.
        waiters = new Queue<TaskCompletionSource>(participants - 1);
    }

    [SuppressMessage("", "SH007")]
    public ValueTask SignalAndWaitAsync()
    {
        lock (waiters)
        {
            if (waiters.Count + 1 == participantCount)
            {
                // This is the last one we were waiting for.
                // Unleash everyone that preceded this one.
                while (waiters.Count > 0)
                {
                    _ = Task.Factory.StartNew(state => ((TaskCompletionSource)state!).SetResult(), waiters.Dequeue(), default, TaskCreationOptions.None, TaskScheduler.Default);
                }

                // And allow this one to continue immediately.
                return ValueTask.CompletedTask;
            }
            else
            {
                // We need more folks. So suspend this caller.
                TaskCompletionSource tcs = new();
                waiters.Enqueue(tcs);
                return tcs.Task.AsValueTask();
            }
        }
    }
}