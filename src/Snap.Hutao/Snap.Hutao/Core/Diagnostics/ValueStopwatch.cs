// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Core.Diagnostics;

/// <summary>
/// 值类型的<see cref="Stopwatch"/>
/// </summary>
internal struct ValueStopwatch
{
    private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

    private readonly long startTimestamp;

    private ValueStopwatch(long startTimestamp)
    {
        this.startTimestamp = startTimestamp;
    }

    /// <summary>
    /// 是否处于活动状态
    /// </summary>
    public bool IsActive
    {
        get => startTimestamp != 0;
    }

    /// <summary>
    /// 触发一个新的停表
    /// </summary>
    /// <returns>一个新的停表实例</returns>
    public static ValueStopwatch StartNew()
    {
        return new(Stopwatch.GetTimestamp());
    }

    /// <summary>
    /// 获取经过的时间
    /// </summary>
    /// <returns>经过的时间</returns>
    /// <exception cref="InvalidOperationException">当前的停表未合理的初始化</exception>
    public TimeSpan GetElapsedTime()
    {
        // Start timestamp can't be zero in an initialized ValueStopwatch.
        // It would have to be literally the first thing executed when the machine boots to be 0.
        // So it being 0 is a clear indication of default(ValueStopwatch)
        Verify.Operation(IsActive, $"An uninitialized, or 'default', {nameof(ValueStopwatch)} cannot be used to get elapsed time.");

        long end = Stopwatch.GetTimestamp();
        long timestampDelta = end - startTimestamp;
        long ticks = (long)(TimestampToTicks * timestampDelta);

        return new TimeSpan(ticks);
    }
}
