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
    public static ValueTask RandomAsync(int minMilliSeconds, int maxMilliSeconds)
    {
        return Task.Delay((int)(Random.Shared.NextDouble() * (maxMilliSeconds - minMilliSeconds)) + minMilliSeconds).AsValueTask();
    }
}