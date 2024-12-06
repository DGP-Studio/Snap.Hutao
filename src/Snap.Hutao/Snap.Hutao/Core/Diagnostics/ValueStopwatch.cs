// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Diagnostics;

internal readonly struct ValueStopwatch
{
    private readonly long startTimestamp;

    private ValueStopwatch(long startTimestamp)
    {
        this.startTimestamp = startTimestamp;
    }

    public bool IsActive
    {
        get => startTimestamp != 0;
    }

    public static ValueStopwatch StartNew()
    {
        return new(Stopwatch.GetTimestamp());
    }

    public static MeasureExecutionToken MeasureExecution(ILogger logger, [CallerMemberName] string callerName = default!)
    {
        return new(StartNew(), logger, callerName);
    }

    public TimeSpan GetElapsedTime()
    {
        return Stopwatch.GetElapsedTime(startTimestamp);
    }
}