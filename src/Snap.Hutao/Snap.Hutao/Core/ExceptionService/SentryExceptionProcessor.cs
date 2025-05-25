// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Sentry.Extensibility;
using Sentry.Protocol;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;

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
                    switch (ex)
                    {
                        case COMException comException:
                            mechanism.Data["ErrorCode"] = $"0x{comException.ErrorCode:X8}";
                            break;
                        case HttpRequestException httpRequestException:
                            mechanism.Data["HttpRequestError"] = $"{httpRequestException.HttpRequestError}";
                            break;
                        case HttpIOException httpIOException:
                            mechanism.Data["HttpRequestError"] = $"{httpIOException.HttpRequestError}";
                            break;
                        case SocketException socketException:
                            mechanism.Data["SocketErrorCode"] = $"{socketException.SocketErrorCode}";
                            break;
                        case Win32Exception win32Exception:
                            mechanism.Data["NativeErrorCode"] = $"0x{win32Exception.NativeErrorCode:X8}";
                            break;
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