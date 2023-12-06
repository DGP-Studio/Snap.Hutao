﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Diagnostics;

/// <summary>
/// 值类型的<see cref="Stopwatch"/>
/// </summary>
internal readonly struct ValueStopwatch
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
    /// 测量运行时间
    /// </summary>
    /// <param name="logger">日志器</param>
    /// <param name="callerName">调用方法名称</param>
    /// <returns>结束测量</returns>
    public static MeasureExecutionToken MeasureExecution(ILogger logger, [CallerMemberName] string callerName = default!)
    {
        ValueStopwatch stopwatch = StartNew();
        return new MeasureExecutionToken(stopwatch, logger, callerName);
    }

    /// <summary>
    /// 获取经过的时间
    /// </summary>
    /// <returns>经过的时间</returns>
    public TimeSpan GetElapsedTime()
    {
        return Stopwatch.GetElapsedTime(startTimestamp);
    }
}