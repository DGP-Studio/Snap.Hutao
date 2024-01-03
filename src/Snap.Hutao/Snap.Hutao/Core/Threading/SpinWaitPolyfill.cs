// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

internal delegate bool SpinWaitPredicate<T>(ref readonly T state);

internal static class SpinWaitPolyfill
{
    public static unsafe void SpinUntil<T>(ref T state, delegate*<ref readonly T, bool> condition)
    {
        SpinWait spinner = default;
        while (!condition(ref state))
        {
            spinner.SpinOnce();
        }
    }
}