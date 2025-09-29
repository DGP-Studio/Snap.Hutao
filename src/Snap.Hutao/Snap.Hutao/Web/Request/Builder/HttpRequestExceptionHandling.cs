// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Web.Endpoint.Hutao;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Authentication;
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
        Uri? uri = builder.RequestUri is null ? default : new UriBuilder(builder.RequestUri).Uri;

        // Hutao Generic API handling
        if (TryHandleGenericApiRequest(messageBuilder, builder, ex, uri))
        {
            return true;
        }

        if (FormatException(messageBuilder, ex, uri?.GetLeftPart(UriPartial.Path)))
        {
            return true;
        }

        // https://github.com/getsentry/sentry-dotnet/blob/main/src/Sentry/SentryHttpFailedRequestHandler.cs
        SentryRequest request = new()
        {
            QueryString = builder.RequestUri?.Query,
            Method = builder.Method.Method.ToUpperInvariant(),
            Url = uri?.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped),
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
            if (networkError is not NetworkError.NULL)
            {
                builder.AppendLine(networkError.ToString());
                switch (networkError)
                {
                    case NetworkError.ERR_SECURE_CONNECTION_AUTHENTICATION_ERROR:
                        builder.AppendLine(ex.InnerException?.Message); // AuthenticationException has more details
                        break;
                    case NetworkError.ERR_UNKNOWN_AUTHENTICATION_ERROR:
                        builder.AppendLine(ex.InnerException?.InnerException?.Message); // AuthenticationException has more details
                        break;
                    default:
                        builder.AppendLine(ex.Message);
                        break;
                }

                return true;
            }

            if (httpRequestException.StatusCode is { } statusCode)
            {
                if ((int)statusCode is < 200 or > 299)
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
                            case SocketError.AccessDenied:
                                return NetworkError.ERR_CONNECTION_ACCESS_DENIED;
                            case SocketError.AddressAlreadyInUse:
                                return NetworkError.ERR_CONNECTION_ADDRESS_ALREADY_IN_USE;
                            case SocketError.AddressNotAvailable:
                                return NetworkError.ERR_CONNECTION_ADDRESS_NOT_AVAILABLE;
                            case SocketError.ConnectionAborted:
                                return NetworkError.ERR_CONNECTION_ABORTED;
                            case SocketError.ConnectionRefused:
                                return NetworkError.ERR_CONNECTION_REFUSED;
                            case SocketError.HostUnreachable:
                                return NetworkError.ERR_CONNECTION_HOST_UNREACHABLE;
                            case SocketError.NetworkUnreachable:
                                return NetworkError.ERR_CONNECTION_NETWORK_UNREACHABLE;
                            case SocketError.NoBufferSpaceAvailable:
                                return NetworkError.ERR_CONNECTION_NO_BUFFER_SPACE_AVAILABLE;
                            case SocketError.NoData:
                                return NetworkError.ERR_CONNECTION_NO_DATA;
                            case SocketError.NotSocket:
                                return NetworkError.ERR_CONNECTION_NOT_SOCKET;
                            case SocketError.TimedOut:
                                return NetworkError.ERR_CONNECTION_TIMED_OUT;
                        }

                        break;
                }

                break;

            case HttpRequestError.NameResolutionError:
                switch (ex.InnerException)
                {
                    case SocketException socketException:
                        switch (socketException.SocketErrorCode)
                        {
                            case SocketError.HostNotFound:
                                return NetworkError.ERR_NAME_RESOLUTION_HOST_NOT_FOUND;
                        }

                        break;
                }

                break;

            case HttpRequestError.ProxyTunnelError:
                {
                    return NetworkError.ERR_PROXY_TUNNEL_ERROR;
                }

            case HttpRequestError.ResponseEnded:
                switch (ex.InnerException)
                {
                    case HttpIOException httpIOException:
                        switch (httpIOException.HttpRequestError)
                        {
                            case HttpRequestError.ResponseEnded:
                                return NetworkError.ERR_RESPONSE_ENDED;
                        }

                        break;
                }

                break;

            case HttpRequestError.SecureConnectionError:
                switch (ex.InnerException)
                {
                    case AuthenticationException authenticationException:
                        {
                            switch (authenticationException.InnerException)
                            {
                                case Win32Exception win32Exception:
                                    switch (win32Exception.NativeErrorCode)
                                    {
                                        // 无法连接到本地安全机构 SEC_E_INTERNAL_ERROR
                                        case unchecked((int)0x80090304):
                                            return NetworkError.ERR_SECURE_CONNECTION_SEC_E_INTERNAL_ERROR;

                                        // 接收到的消息异常，或格式不正确 SEC_E_ILLEGAL_MESSAGE
                                        case unchecked((int)0x80090326):
                                            return NetworkError.ERR_SECURE_CONNECTION_SEC_E_ILLEGAL_MESSAGE;
                                    }

                                    break;

                                case null:
                                    return NetworkError.ERR_SECURE_CONNECTION_AUTHENTICATION_ERROR;
                            }

                            break;
                        }

                    case IOException ioException:
                        {
                            switch (ioException.InnerException)
                            {
                                case SocketException socketException:
                                    switch (socketException.SocketErrorCode)
                                    {
                                        case SocketError.ConnectionAborted:
                                            return NetworkError.ERR_SECURE_CONNECTION_ABORTED;
                                        case SocketError.ConnectionReset:
                                            return NetworkError.ERR_SECURE_CONNECTION_RESET;
                                    }

                                    break;

                                case null:
                                    return NetworkError.ERR_SECURE_CONNECTION_ERROR;
                            }
                        }

                        break;
                }

                break;

            case HttpRequestError.Unknown:
                switch (ex.InnerException)
                {
                    case IOException ioException:
                        switch (ioException.InnerException)
                        {
                            case AuthenticationException authenticationException:
                                switch (authenticationException.HResult)
                                {
                                    // COR_E_SYSTEM Cannot determine the frame size or a corrupted frame was received.
                                    case unchecked((int)0x80131501):
                                        return NetworkError.ERR_UNKNOWN_AUTHENTICATION_ERROR;
                                }

                                break;

                            case SocketException socketException:
                                switch (socketException.SocketErrorCode)
                                {
                                    case SocketError.AccessDenied:
                                        return NetworkError.ERR_UNKNOWN_ACCESS_DENIED;
                                    case SocketError.ConnectionAborted:
                                        return NetworkError.ERR_UNKNOWN_CONNECTION_ABORTED;
                                    case SocketError.ConnectionReset:
                                        return NetworkError.ERR_UNKNOWN_CONNECTION_RESET;
                                    case SocketError.TimedOut:
                                        return NetworkError.ERR_UNKNOWN_TIMED_OUT;
                                }

                                break;

                            case Win32Exception win32Exception:
                                switch (win32Exception.NativeErrorCode)
                                {
                                    // 为验证提供的消息或签名已被更改 SEC_E_MESSAGE_ALTERED
                                    case unchecked((int)0x8009030F):
                                        return NetworkError.ERR_UNKNOWN_SEC_E_MESSAGE_ALTERED;

                                    // 无法解密指定的数据 SEC_E_DECRYPT_FAILURE
                                    case unchecked((int)0x80090330):
                                        return NetworkError.ERR_UNKNOWN_SEC_E_DECRYPT_FAILURE;
                                }

                                break;

                            case null:
                                return NetworkError.ERR_UNKNOWN;
                        }

                        break;
                }

                break;
        }

        return NetworkError.NULL;
    }

    private static bool TryHandleGenericApiRequest(StringBuilder messageBuilder, HttpRequestMessageBuilder builder, Exception ex, Uri? uri)
    {
        IInfrastructureEndpoints endpoints = builder.ServiceProvider.GetRequiredService<IHutaoEndpointsFactory>().Create();
        if (uri is not null && string.Equals(uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped), endpoints.Root, StringComparison.OrdinalIgnoreCase))
        {
            if (ex is HttpRequestException { StatusCode: (HttpStatusCode)418 or HttpStatusCode.UnavailableForLegalReasons })
            {
                messageBuilder.AppendLine(SH.ServiceMetadataVersionNotSupported);
                return true;
            }
        }

        return false;
    }
}