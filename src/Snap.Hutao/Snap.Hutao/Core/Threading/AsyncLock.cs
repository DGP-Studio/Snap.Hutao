// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-6-asynclock/
[SuppressMessage("", "SH003")]
internal sealed class AsyncLock
{
    private readonly AsyncSemaphore semaphore;
    private readonly Task<Releaser> releaser;

    public AsyncLock()
    {
        semaphore = new AsyncSemaphore(1, 1);
        releaser = Task.FromResult(new Releaser(this));
    }

    [SuppressMessage("", "SH007")]
    public Task<Releaser> LockAsync()
    {
        Task wait = semaphore.WaitAsync();
        return wait.IsCompleted ? releaser : wait.ContinueWith((_, state) => new Releaser((AsyncLock)state!), this, default, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }

    internal readonly struct Releaser : IDisposable
    {
        private readonly AsyncLock toRelease;

        internal Releaser(AsyncLock toRelease)
        {
            this.toRelease = toRelease;
        }

        public readonly void Dispose()
        {
            toRelease.semaphore.Release();
        }
    }
}