// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Threading;

internal sealed class Throttler
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> methodSemaphoreMap = new();

    public ValueTask<SemaphoreSlimToken> ThrottleAsync(CancellationToken token = default, [CallerMemberName] string callerName = default!, [CallerLineNumber] int callerLine = 0)
    {
        string key = $"{callerName}L{callerLine}";
        SemaphoreSlim semaphore = methodSemaphoreMap.GetOrAdd(key, name => new SemaphoreSlim(1));
        return semaphore.EnterAsync(token);
    }
}