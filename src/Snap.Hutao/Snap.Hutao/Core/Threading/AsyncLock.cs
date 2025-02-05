// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-6-asynclock/
[SuppressMessage("", "SH003")]
internal sealed class AsyncLock
{
    private static readonly Func<Task, object?, Releaser> Continuation = RunContinuation;

    private readonly AsyncSemaphore semaphore;
    private readonly Task<Releaser> releaser;

    public AsyncLock()
    {
        semaphore = new(1, 1);
        releaser = Task.FromResult(new Releaser(this));
    }

    [SuppressMessage("", "SH007")]
    public Task<Releaser> LockAsync()
    {
        Task wait = semaphore.WaitAsync();
        return wait.IsCompleted ? releaser : wait.ContinueWith(Continuation, this, default, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }

    public bool TryLock(out Releaser releaser)
    {
        if (semaphore.TryWait())
        {
            releaser = new(this);
            return true;
        }

        releaser = default;
        return false;
    }

    private static Releaser RunContinuation(Task task, object? state)
    {
        ArgumentNullException.ThrowIfNull(state);
        return new((AsyncLock)state);
    }

    internal readonly struct Releaser : IDisposable
    {
        private readonly AsyncLock toRelease;

        internal Releaser(AsyncLock toRelease)
        {
            this.toRelease = toRelease;
        }

        public void Dispose()
        {
            toRelease.semaphore.Release();
        }
    }
}