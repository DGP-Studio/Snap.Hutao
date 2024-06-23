// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading;

#if NET9_0_OR_GREATER
[Obsolete]
#endif
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
}