// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-7-asyncreaderwriterlock/
[SuppressMessage("", "SH003")]
internal sealed class AsyncReaderWriterLock
{
    private readonly Task<Releaser> readerReleaser;
    private readonly Task<Releaser> writerReleaser;

    private readonly Queue<TaskCompletionSource<Releaser>> waitingWriters = [];
    private TaskCompletionSource<Releaser> waitingReader = new();
    private int readersWaiting;
    private int status; // readersReading or -1 if a writer is writing

    public AsyncReaderWriterLock()
    {
        readerReleaser = Task.FromResult(new Releaser(this, false));
        writerReleaser = Task.FromResult(new Releaser(this, true));
    }

    [SuppressMessage("", "CA2008")]
    public Task<Releaser> ReaderLockAsync()
    {
        lock (waitingWriters)
        {
            if (status >= 0 && waitingWriters.Count == 0)
            {
                ++status;
                return readerReleaser;
            }

            ++readersWaiting;

            // Cannot await in the body of a lock statement
            return waitingReader.Task.ContinueWith(t => t.Result);
        }
    }

    public Task<Releaser> WriterLockAsync()
    {
        lock (waitingWriters)
        {
            if (status == 0)
            {
                status = -1;
                return writerReleaser;
            }

            TaskCompletionSource<Releaser> waiter = new();
            waitingWriters.Enqueue(waiter);
            return waiter.Task;
        }
    }

    private void ReaderRelease()
    {
        TaskCompletionSource<Releaser>? toWake = default;

        lock (waitingWriters)
        {
            --status;
            if (status == 0 && waitingWriters.Count > 0)
            {
                status = -1;
                toWake = waitingWriters.Dequeue();
            }
        }

        toWake?.SetResult(new(this, true));
    }

    private void WriterRelease()
    {
        TaskCompletionSource<Releaser>? toWake = null;
        bool toWakeIsWriter = false;

        lock (waitingWriters)
        {
            if (waitingWriters.Count > 0)
            {
                toWake = waitingWriters.Dequeue();
                toWakeIsWriter = true;
            }
            else if (readersWaiting > 0)
            {
                toWake = waitingReader;
                status = readersWaiting;
                readersWaiting = 0;
                waitingReader = new();
            }
            else
            {
                status = 0;
            }
        }

        toWake?.SetResult(new(this, toWakeIsWriter));
    }

    internal readonly struct Releaser : IDisposable
    {
        private readonly AsyncReaderWriterLock toRelease;
        private readonly bool writer;

        internal Releaser(AsyncReaderWriterLock toRelease, bool writer)
        {
            this.toRelease = toRelease;
            this.writer = writer;
        }

        public void Dispose()
        {
            if (writer)
            {
                toRelease.WriterRelease();
            }
            else
            {
                toRelease.ReaderRelease();
            }
        }
    }
}