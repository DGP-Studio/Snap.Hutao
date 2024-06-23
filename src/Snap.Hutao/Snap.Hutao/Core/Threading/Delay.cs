// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

[Obsolete]
internal readonly struct Delay
{
    /// <summary>
    /// 随机延迟
    /// </summary>
    /// <param name="min">最小，闭</param>
    /// <param name="max">最小，开</param>
    /// <returns>任务</returns>
    public static ValueTask RandomMilliSeconds(int min, int max)
    {
        return Task.Delay((int)(System.Random.Shared.NextDouble() * (max - min)) + min).AsValueTask();
    }

    public static ValueTask FromSeconds(int seconds)
    {
        return Task.Delay(TimeSpan.FromSeconds(seconds)).AsValueTask();
    }

    public static ValueTask FromMilliSeconds(int seconds)
    {
        return Task.Delay(TimeSpan.FromMilliseconds(seconds)).AsValueTask();
    }
}