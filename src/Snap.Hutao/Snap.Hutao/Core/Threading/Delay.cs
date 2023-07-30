// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

internal readonly struct Delay
{
    /// <summary>
    /// 随机延迟
    /// </summary>
    /// <param name="minMilliSeconds">最小，闭</param>
    /// <param name="maxMilliSeconds">最小，开</param>
    /// <returns>任务</returns>
    [SuppressMessage("", "VSTHRD200")]
    public static ValueTask Random(int minMilliSeconds, int maxMilliSeconds)
    {
        return Task.Delay((int)(System.Random.Shared.NextDouble() * (maxMilliSeconds - minMilliSeconds)) + minMilliSeconds).AsValueTask();
    }

    [SuppressMessage("", "VSTHRD200")]
    public static ValueTask FromSeconds(int seconds)
    {
        return Task.Delay(TimeSpan.FromSeconds(seconds)).AsValueTask();
    }
}