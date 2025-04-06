// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

namespace Snap.Hutao.Web.Request.Builder;

internal static class HttpRequestExceptionHandling
{
    public static bool TryHandle(HttpRequestMessageBuilder builder, Exception ex, out StringBuilder message)
    {
        message = new();
        return TryHandle(message, builder, ex);
    }

    public static bool TryHandle(StringBuilder messageBuilder, HttpRequestMessageBuilder builder, Exception ex)
    {
        if (FormatException(messageBuilder, ex, builder.RequestUri is null ? default : new UriBuilder(builder.RequestUri).Uri.GetLeftPart(UriPartial.Path)))
        {
            return true;
        }

        // https://github.com/getsentry/sentry-dotnet/blob/main/src/Sentry/SentryHttpFailedRequestHandler.cs
        SentryRequest request = new()
        {
            QueryString = builder.RequestUri?.Query,
            Method = builder.Method.Method.ToUpperInvariant(),
            Url = builder.RequestUri is null ? default : new UriBuilder(builder.RequestUri).Uri.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped),
        };

        SentrySdk.CaptureException(ex, scope =>
        {
            scope.Request = request;

            if (ExceptionAttachment.TryGetAttachment(ex, out SentryAttachment? attachment))
            {
                scope.AddAttachment(attachment);
            }
        });

        return false;
    }

    public static bool FormatException(StringBuilder builder, Exception ex, string? url)
    {
        if (ex is HttpRequestException httpRequestException)
        {
            builder.AppendLine(SH.FormatWebRequestBuilderExceptionDescription(url));

            NetworkError networkError = HttpRequestExceptionToNetworkError(httpRequestException);
            if (networkError is not NetworkError.OK)
            {
                builder.AppendLine(networkError.ToString());
                builder.AppendLine(ex.Message);
                return true;
            }

            if (httpRequestException.StatusCode is { } statusCode)
            {
                if (((int)statusCode) is (< 200 or > 299))
                {
                    builder.Append("HTTP ").Append((int)statusCode);
                    if (Enum.IsDefined(statusCode))
                    {
                        builder.Append(' ').Append(statusCode.ToString());
                    }

                    return true;
                }
            }
        }

        ExceptionFormat.Format(builder, ex);
        return false;
    }

    public static NetworkError HttpRequestExceptionToNetworkError(HttpRequestException ex)
    {
        switch (ex.HttpRequestError)
        {
            case HttpRequestError.ConnectionError:
                switch (ex.InnerException)
                {
                    case SocketException socketException:
                        switch (socketException.SocketErrorCode)
                        {
                            case SocketError.ConnectionRefused:
                                return NetworkError.ERR_CONNECTION_REFUSED;
                            case SocketError.TimedOut:
                                return NetworkError.ERR_CONNECTION_TIMED_OUT;
                        }

                        break;
                }

                break;

            case HttpRequestError.SecureConnectionError:
                switch (ex.InnerException)
                {
                    case IOException ioException:
                        {
                            switch (ioException.InnerException)
                            {
                                case SocketException socketException:
                                    switch (socketException.SocketErrorCode)
                                    {
                                        case SocketError.ConnectionAborted:
                                            return NetworkError.ERR_CONNECTION_ABORTED;
                                    }

                                    break;
                            }
                        }

                        break;
                }

                break;
        }

        return NetworkError.OK;
    }
}