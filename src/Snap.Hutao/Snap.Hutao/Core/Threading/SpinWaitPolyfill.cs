// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Core.Threading;

internal static class SpinWaitPolyfill
{
    public static unsafe void SpinUntil<T>(ref readonly T state, delegate*<ref readonly T, bool> condition)
    {
        SpinWait spinner = default;
        while (!condition(in state))
        {
            spinner.SpinOnce();
        }
    }

    public static unsafe bool SpinUntil<T>(ref readonly T state, delegate*<ref readonly T, bool> condition, TimeSpan timeout)
    {
        long startTime = Stopwatch.GetTimestamp();

        SpinWait spinner = default;
        while (!condition(in state))
        {
            spinner.SpinOnce();

            if (timeout < Stopwatch.GetElapsedTime(startTime))
            {
                return false;
            }
        }

        return true;
    }
}