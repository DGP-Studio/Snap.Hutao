// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Core.Threading;

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

    [SuppressMessage("", "SH002")]
    public static unsafe bool SpinUntil<T>(ref T state, delegate*<ref readonly T, bool> condition, TimeSpan timeout)
    {
        long startTime = Stopwatch.GetTimestamp();

        SpinWait spinner = default;
        while (!condition(ref state))
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