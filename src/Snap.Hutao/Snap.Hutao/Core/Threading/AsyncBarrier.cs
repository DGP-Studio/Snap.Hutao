// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

/// <summary>
/// An asynchronous barrier that blocks the signaler until all other participants have signaled.
/// FIFO
/// </summary>
internal class AsyncBarrier
{
    /// <summary>
    /// The number of participants being synchronized.
    /// </summary>
    private readonly int participantCount;

    /// <summary>
    /// The set of participants who have reached the barrier, with their awaiters that can resume those participants.
    /// </summary>
    private readonly Queue<TaskCompletionSource> waiters;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncBarrier"/> class.
    /// </summary>
    /// <param name="participants">The number of participants.</param>
    public AsyncBarrier(int participants)
    {
        Must.Range(participants >= 1, "Participants of AsyncBarrier can not be less than 1");
        participantCount = participants;

        // Allocate the stack so no resizing is necessary.
        // We don't need space for the last participant, since we never have to store it.
        waiters = new Queue<TaskCompletionSource>(participants - 1);
    }

    /// <summary>
    /// Signals that a participant is ready, and returns a Task
    /// that completes when all other participants have also signaled ready.
    /// </summary>
    /// <returns>A Task, which will complete (or may already be completed) when the last participant calls this method.</returns>
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