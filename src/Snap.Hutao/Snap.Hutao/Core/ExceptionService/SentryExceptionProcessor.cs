// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Sentry.Extensibility;
using Sentry.Protocol;
using System.Net.Http;

namespace Snap.Hutao.Core.ExceptionService;

internal sealed class SentryExceptionProcessor : ISentryEventExceptionProcessor
{
    public void Process(Exception exception, SentryEvent sentryEvent)
    {
        if (sentryEvent.SentryExceptions is { } exceptions)
        {
            foreach ((SentryException sentryEx, Exception ex) in exceptions.Reverse().Zip(WalkExceptions(exception)))
            {
                if (sentryEx.Mechanism is { } mechanism)
                {
                    mechanism.Data["HResult"] = $"0x{ex.HResult:X8}";
                    if (ex is HttpRequestException httpRequestException)
                    {
                        mechanism.Data["HttpRequestError"] = $"{httpRequestException.HttpRequestError}";
                    }
                }
            }
        }
    }

    private static IEnumerable<Exception> WalkExceptions(Exception exception)
    {
        Exception? currentException = exception;
        while (currentException is not null)
        {
            yield return currentException;

            if (currentException is AggregateException aggregateException)
            {
                foreach (Exception innerException in aggregateException.InnerExceptions)
                {
                    foreach (Exception exception2 in WalkExceptions(innerException))
                    {
                        yield return exception2;
                    }
                }
            }

            currentException = currentException.InnerException;
        }
    }
}