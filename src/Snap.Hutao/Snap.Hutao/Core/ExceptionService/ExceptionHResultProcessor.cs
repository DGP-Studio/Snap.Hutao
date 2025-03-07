// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Sentry.Extensibility;
using Sentry.Protocol;

namespace Snap.Hutao.Core.ExceptionService;

internal sealed class ExceptionHResultProcessor : ISentryEventExceptionProcessor
{
    public void Process(Exception exception, SentryEvent sentryEvent)
    {
        List<string> results = [];
        WalkExceptions(exception, results);

        if (sentryEvent.SentryExceptions is { } exceptions)
        {
            foreach ((SentryException ex, string hr) in exceptions.Reverse().Zip(results))
            {
                if (ex.Mechanism is { } mechanism)
                {
                    mechanism.Data["HResult"] = hr;
                }
            }
        }
    }

    private static void WalkExceptions(Exception exception, List<string> results)
    {
        Exception? currentException = exception;
        while (currentException is not null)
        {
            results.Add($"0x{currentException.HResult:X8}");

            if (currentException is AggregateException aggregateException)
            {
                foreach (Exception innerException in aggregateException.InnerExceptions)
                {
                    WalkExceptions(innerException, results);
                }
            }

            currentException = currentException.InnerException;
        }
    }
}