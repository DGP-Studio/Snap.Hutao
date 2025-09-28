// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-7-asyncreaderwriterlock/
[SuppressMessage("", "SH003")]
internal sealed class AsyncReaderWriterLock
{
    private readonly List<string> traces = [];
    private readonly Queue<(string Trace, TaskCompletionSource<Releaser> Tcs)> waitingWriters = [];
    private TaskCompletionSource<Releaser> waitingReader = new();
    private int readersWaiting;
    private int status; // readersReading or -1 if a writer is writing

    [SuppressMessage("", "CA2008")]
    public Task<Releaser> ReaderLockAsync(string trace)
    {
        lock (waitingWriters)
        {
            traces.Add($"{trace} request as reader");
            if (status >= 0 && waitingWriters.Count == 0)
            {
                ++status;
                traces.Add($"{trace} enter as reader");
                return Task.FromResult(new Releaser(this, trace, false));
            }

            ++readersWaiting;

            // Cannot await in the body of a lock statement
            return waitingReader.Task.ContinueWith(t =>
            {
                traces.Add($"{trace} enter as reader");
                return t.Result;
            });
        }
    }

    public bool TryReaderLock(string trace, out Releaser releaser)
    {
        lock (waitingWriters)
        {
            traces.Add($"{trace} request as reader");
            if (status >= 0 && waitingWriters.Count == 0)
            {
                ++status;
                traces.Add($"{trace} enter as reader");
                releaser = new(this, trace, false);
                return true;
            }
        }

        traces.Add($"{trace} fail to enter as reader");
        releaser = default;
        return false;
    }

    [SuppressMessage("", "CA2008")]
    public Task<Releaser> WriterLockAsync(string trace)
    {
        lock (waitingWriters)
        {
            traces.Add($"{trace} request as writer");
            if (status == 0)
            {
                status = -1;
                traces.Add($"{trace} enter as writer");
                return Task.FromResult(new Releaser(this, trace, true));
            }

            TaskCompletionSource<Releaser> waiter = new();
            waitingWriters.Enqueue((trace, waiter));
            return waiter.Task.ContinueWith(t =>
            {
                traces.Add($"{trace} enter as writer");
                return t.Result;
            });
        }
    }

    public bool TryWriterLock(string trace, out Releaser releaser)
    {
        lock (waitingWriters)
        {
            traces.Add($"{trace} request as writer");
            if (status == 0)
            {
                status = -1;
                traces.Add($"{trace} enter as writer");
                releaser = new(this, trace, true);
                return true;
            }
        }

        traces.Add($"{trace} fail to enter as writer");
        releaser = default;
        return false;
    }

    public override string ToString()
    {
        return $"""
            Traces: 
            {string.Join("\r\n", traces)}
            """;
    }

    private void ReaderRelease()
    {
        TaskCompletionSource<Releaser>? toWake = default;
        string trace = default!;

        lock (waitingWriters)
        {
            --status;
            if (status == 0 && waitingWriters.Count > 0)
            {
                status = -1;
                (trace, toWake) = waitingWriters.Dequeue();
            }
        }

        toWake?.SetResult(new(this, trace, true));
    }

    private void WriterRelease()
    {
        TaskCompletionSource<Releaser>? toWake = null;
        string trace = default!;
        bool toWakeIsWriter = false;

        lock (waitingWriters)
        {
            if (waitingWriters.Count > 0)
            {
                (trace, toWake) = waitingWriters.Dequeue();
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

        toWake?.SetResult(new(this, trace, toWakeIsWriter));
    }

    internal readonly struct Releaser : IDisposable
    {
        private readonly AsyncReaderWriterLock toRelease;
        private readonly string trace;
        private readonly bool writer;

        internal Releaser(AsyncReaderWriterLock toRelease, string trace, bool writer)
        {
            this.toRelease = toRelease;
            this.trace = trace;
            this.writer = writer;
        }

        public void Dispose()
        {
            toRelease.traces.Add($"{trace} release as {(writer ? "writer" : "reader")}");
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