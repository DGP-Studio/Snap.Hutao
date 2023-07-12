// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
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

    public void Dispose()
    {
        logger.LogInformation("{Caller} toke {Time} ms", callerName, stopwatch.GetElapsedTime().TotalMilliseconds);
    }
}