// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Concurrent;

namespace Snap.Hutao.Core.Threading;

[SuppressMessage("", "SH003")]
internal sealed class AsyncKeyedLock<TKey>
    where TKey : notnull
{
    private static readonly Func<Task, object?, Releaser> Continuation = RunContinuation;

    private readonly ConcurrentDictionary<TKey, AsyncSemaphore> semaphores;

    public AsyncKeyedLock()
    {
        semaphores = [];
    }

    public AsyncKeyedLock(IEqualityComparer<TKey>? comparer)
    {
        semaphores = new(comparer);
    }

    public Task<Releaser> LockAsync(TKey key)
    {
        lock (semaphores)
        {
            Task wait = semaphores.GetOrAdd(key, _ => new(1, 1)).WaitAsync();
            State stateObj = new(this, key);
            return wait.IsCompleted ? Task.FromResult<Releaser>(new(stateObj)) : wait.ContinueWith(Continuation, stateObj, default, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }
    }

    private static Releaser RunContinuation(Task task, object? state)
    {
        ArgumentNullException.ThrowIfNull(state);
        return new((State)state);
    }

    internal readonly struct Releaser : IDisposable
    {
        private readonly State state;

        internal Releaser(State state)
        {
            this.state = state;
        }

        public void Dispose()
        {
            lock (state.ToRelease.semaphores)
            {
                if (state.ToRelease.semaphores.TryGetValue(state.Key, out AsyncSemaphore? semaphore))
                {
                    if (semaphore.Release() is 1)
                    {
                        state.ToRelease.semaphores.TryRemove(state.Key, out _);
                    }
                }
            }
        }
    }

    internal sealed class State
    {
        public State(AsyncKeyedLock<TKey> toRelease, TKey key)
        {
            ToRelease = toRelease;
            Key = key;
        }

        public AsyncKeyedLock<TKey> ToRelease { get; }

        public TKey Key { get; }
    }
}