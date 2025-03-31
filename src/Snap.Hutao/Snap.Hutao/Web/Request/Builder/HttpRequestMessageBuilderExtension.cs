// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Response;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Snap.Hutao.Web.Request.Builder;

internal static class HttpRequestMessageBuilderExtension
{
    internal static HttpRequestMessageBuilder Resurrect(this HttpRequestMessageBuilder builder)
    {
        builder.HttpRequestMessage.Resurrect();
        return builder;
    }

    internal static async ValueTask<TypedHttpResponse<TResult>> SendAsync<TResult>(this HttpRequestMessageBuilder builder, HttpClient httpClient, CancellationToken token)
        where TResult : class
    {
        HttpContext context = new()
        {
            HttpClient = httpClient,
            RequestAborted = token,
        };

        using (context)
        {
            await SendAsync(builder, context).ConfigureAwait(false);

            StringBuilder messageBuilder = new();
            bool showInfo = true;

            try
            {
                context.Exception?.Throw();
                ArgumentNullException.ThrowIfNull(context.Response);

                showInfo = false;
                TResult? body = await builder.HttpContentSerializer.DeserializeAsync<TResult>(context.Response.Content, token).ConfigureAwait(false);
                return new(context.Response.Headers, body);
            }
            catch (OperationCanceledException)
            {
                showInfo = false;

                // Populate to caller
                throw;
            }
            catch (Exception ex)
            {
                if (!FormatException(messageBuilder, ex, builder.RequestUri is null ? default : new UriBuilder(builder.RequestUri).Uri.GetLeftPart(UriPartial.Path)))
                {
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
                }

                return new(context.Response?.Headers, default);
            }
            finally
            {
                if (showInfo)
                {
                    builder.ServiceProvider.GetRequiredService<IInfoBarService>().Error(messageBuilder.ToString());
                }
            }
        }
    }

    internal static async ValueTask SendAsync(this HttpRequestMessageBuilder builder, HttpContext context)
    {
        string? baseUrl = builder.HttpRequestMessage.RequestUri?.GetLeftPart(UriPartial.Path);
        try
        {
            context.Request = builder.HttpRequestMessage;
            context.Response = await context.HttpClient.SendAsync(context.Request, context.RequestAborted).ConfigureAwait(false);
            context.Response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            ExceptionFingerprint.SetFingerprint(ex, baseUrl);
            context.Exception = ExceptionDispatchInfo.Capture(ex);
        }
    }

    internal static void Send(this HttpRequestMessageBuilder builder, HttpClient httpClient, ILogger logger)
    {
        try
        {
            using (httpClient.Send(builder.HttpRequestMessage))
            {
            }
        }
        catch (HttpRequestException)
        {
        }
        catch (IOException)
        {
        }
        catch (JsonException)
        {
        }
        catch (HttpContentSerializationException)
        {
        }
        catch (SocketException)
        {
        }
    }

    private static bool FormatException(StringBuilder builder, Exception ex, string? url)
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
            else
            {
                if (httpRequestException.StatusCode is { } statusCode)
                {
                    if (((int)statusCode) is (< 200 or > 299))
                    {
                        builder.Append("HTTP ").Append((int)statusCode);
                        if (Enum.IsDefined(statusCode))
                        {
                            builder.Append(' ').Append(statusCode);
                        }

                        return true;
                    }
                }
            }
        }

        ExceptionFormat.Format(builder, ex);
        return false;
    }

    private static NetworkError HttpRequestExceptionToNetworkError(HttpRequestException ex)
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