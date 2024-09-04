// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

internal sealed partial class ScopedTaskCompletionSource : IDisposable
{
    private readonly TaskCompletionSource tcs = new();

    public Task Task { get => tcs.Task; }

    public void Dispose()
    {
        tcs.TrySetResult();
    }
}