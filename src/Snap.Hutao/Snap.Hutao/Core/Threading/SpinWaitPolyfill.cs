// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

internal static class SpinWaitPolyfill
{
    public static void SpinUntil<T>(T state, Func<T, bool> condition)
    {
        SpinWait spinner = default;
        while (!condition(state))
        {
            spinner.SpinOnce();
        }
    }
}