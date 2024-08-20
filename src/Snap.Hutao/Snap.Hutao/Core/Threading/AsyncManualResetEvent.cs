// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

// https://devblogs.microsoft.com/pfxteam/building-async-coordination-primitives-part-1-asyncmanualresetevent/
[SuppressMessage("", "SH003")]
internal sealed class AsyncManualResetEvent
{
    private volatile TaskCompletionSource tcs = new();

    public Task WaitAsync()
    {
        return tcs.Task;
    }

    [SuppressMessage("", "SH007")]
    public void Set()
    {
        TaskCompletionSource tcs = this.tcs;
        Task.Factory.StartNew(s => ((TaskCompletionSource)s!).TrySetResult(), tcs, CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default);
        tcs.Task.Wait();
    }

    public void Reset()
    {
        while (true)
        {
            TaskCompletionSource tcs = this.tcs;
            if (!tcs.Task.IsCompleted || Interlocked.CompareExchange(ref this.tcs, new TaskCompletionSource(), tcs) == tcs)
            {
                return;
            }
        }
    }
}