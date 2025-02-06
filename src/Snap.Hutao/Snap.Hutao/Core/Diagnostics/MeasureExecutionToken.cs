// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Diagnostics;

internal readonly struct MeasureExecutionToken : IDisposable
{
    private readonly ValueStopwatch stopwatch;
    private readonly ILogger logger;
    private readonly string callerName;

    public MeasureExecutionToken(in ValueStopwatch stopwatch, ILogger logger, string callerName)
    {
        this.stopwatch = stopwatch;
        this.logger = logger;
        this.callerName = callerName;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        logger.LogDebug("\e[1m\e[33m{Caller} \e[37mtoke \e[32m{Time} \e[37mms", callerName, stopwatch.GetElapsedTime().TotalMilliseconds);
    }
}