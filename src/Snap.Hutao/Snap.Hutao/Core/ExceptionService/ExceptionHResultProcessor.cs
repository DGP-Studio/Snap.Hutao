// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Sentry.Extensibility;

namespace Snap.Hutao.Core.ExceptionService;

internal sealed class ExceptionHResultProcessor : ISentryEventExceptionProcessor
{
    public void Process(Exception exception, SentryEvent sentryEvent)
    {
        List<string?> results = [];
        WalkExceptions(exception, results);
        sentryEvent.SetExtra("HResults", results);
    }

    private static void WalkExceptions(Exception exception, List<string?> results)
    {
        Exception? currentException = exception;
        while (currentException is not null)
        {
            if (currentException.HResult is not 0)
            {
                results.Add($"0x{currentException.HResult:X8}");
            }
            else
            {
                results.Add(null);
            }

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