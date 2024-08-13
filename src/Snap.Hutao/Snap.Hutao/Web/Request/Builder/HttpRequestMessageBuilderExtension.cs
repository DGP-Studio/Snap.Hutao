// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
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
    private const string RequestErrorMessage = "请求异常已忽略: {Uri}";

    internal static HttpRequestMessageBuilder Resurrect(this HttpRequestMessageBuilder builder)
    {
        builder.HttpRequestMessage.Resurrect();
        return builder;
    }

    internal static async ValueTask<TypedHttpResponse<TResult>> SendAsync<TResult>(this HttpRequestMessageBuilder builder, HttpClient httpClient, ILogger logger, CancellationToken token)
        where TResult : class
    {
        HttpContext context = new()
        {
            HttpClient = httpClient,
            Logger = logger,
            RequestAborted = token,
        };

        using (context)
        {
            await SendAsync(builder, context).ConfigureAwait(false);

            StringBuilder messageBuilder = new();
            messageBuilder.AppendLine(System.Globalization.CultureInfo.CurrentCulture, $"Host: {context.Request?.RequestUri?.Host ?? "Unknown"}");
            bool showInfo = true;

            try
            {
                context.Exception?.Throw();
                ArgumentNullException.ThrowIfNull(context.Response);

                context.Response.EnsureSuccessStatusCode();
                showInfo = false;
                TResult? body = await builder.HttpContentSerializer.DeserializeAsync<TResult>(context.Response.Content, token).ConfigureAwait(false);
                return new(context.Response.Headers, body);
            }
            catch (OperationCanceledException)
            {
                showInfo = false;
                throw;
            }
            catch (Exception ex)
            {
                ProcessException(messageBuilder, ex);
                logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
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
        try
        {
            context.Request = builder.HttpRequestMessage;
            context.Response = await context.HttpClient.SendAsync(context.Request, context.RequestAborted).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            context.Exception = ExceptionDispatchInfo.Capture(ex);
            context.Logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
    }

    internal static void Send(this HttpRequestMessageBuilder builder, HttpClient httpClient, ILogger logger)
    {
        try
        {
            using (HttpResponseMessage message = httpClient.Send(builder.HttpRequestMessage))
            {
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
        catch (IOException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
        catch (HttpContentSerializationException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
        catch (SocketException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
    }

    [SuppressMessage("", "CA1305")]
    private static void ProcessException(StringBuilder builder, Exception exception, int depth = 0)
    {
        if (exception is HttpRequestException hre)
        {
            builder
                .AppendLine($"{nameof(HttpRequestException)}: Status Code: {hre.StatusCode} Error: {hre.HttpRequestError}")
                .AppendLine(hre.Message);
        }
        else if (exception is IOException ioe)
        {
            builder
                .AppendLine($"{nameof(IOException)}: 0x{ioe.HResult:X8}")
                .AppendLine(ioe.Message);
        }
        else if (exception is JsonException je)
        {
            builder
                .AppendLine($"{nameof(JsonException)}: Path: {je.Path} at Line: {je.LineNumber} Position: {je.BytePositionInLine}")
                .AppendLine(je.Message);
        }
        else if (exception is HttpContentSerializationException hcse)
        {
            builder
                .AppendLine($"{nameof(HttpContentSerializationException)}:")
                .AppendLine(hcse.Message);
        }
        else if (exception is SocketException se)
        {
            builder
                .AppendLine($"{nameof(SocketException)}: Error: {se.SocketErrorCode}")
                .AppendLine(se.Message);
        }
        else
        {
            builder
                .AppendLine($"{TypeNameHelper.GetTypeDisplayName(exception, false)}:")
                .AppendLine(exception.Message);
        }

        if (exception.InnerException is { } inner)
        {
            builder.AppendLine("------------------ Inner Exception ------------------");
            ProcessException(builder, inner, depth + 1);
        }

        if (depth is 0)
        {
            builder.AppendLine("------------------ End ------------------");
        }
    }
}