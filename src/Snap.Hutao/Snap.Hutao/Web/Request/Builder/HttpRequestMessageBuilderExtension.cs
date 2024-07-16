// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Response;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
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

    internal static async ValueTask<TResult?> SendAsync<TResult>(this HttpRequestMessageBuilder builder, HttpClient httpClient, ILogger logger, CancellationToken token)
        where TResult : class
    {
        StringBuilder messageBuilder = new();
        messageBuilder.AppendLine(System.Globalization.CultureInfo.CurrentCulture, $"Host: {builder.RequestUri?.Host}");
        bool showInfo = true;

        try
        {
            using (builder.HttpRequestMessage)
            {
                using (HttpResponseMessage message = await httpClient.SendAsync(builder.HttpRequestMessage, token).ConfigureAwait(false))
                {
                    message.EnsureSuccessStatusCode();
                    showInfo = false;
                    return result = await builder.HttpContentSerializer.DeserializeAsync<TResult>(message.Content, token).ConfigureAwait(false);
                }
            }
        }
        catch (HttpRequestException ex)
        {
            if (TryHandleHttp502HutaoResponseSpecialCase(ex, out TResult? result))
            {
                return result;
            }

            ProcessException(messageBuilder, ex);
            logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
        catch (IOException ex)
        {
            ProcessException(messageBuilder, ex);
            logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
        catch (JsonException ex)
        {
            ProcessException(messageBuilder, ex);
            logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
        catch (HttpContentSerializationException ex)
        {
            ProcessException(messageBuilder, ex);
            logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
        catch (SocketException ex)
        {
            ProcessException(messageBuilder, ex);
            logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
        finally
        {
            if (showInfo)
            {
                builder.ServiceProvider.GetRequiredService<IInfoBarService>().Error(messageBuilder.ToString());
            }
        }

        return default;
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

    private static bool TryHandleHttp502HutaoResponseSpecialCase<TResult>(HttpRequestException ex, out TResult? result)
        where TResult : class
    {
        result = default;

        if (ex.StatusCode is HttpStatusCode.BadGateway)
        {
            Type resultType = typeof(TResult);

            if (resultType == typeof(HutaoResponse))
            {
                // HutaoResponse(int returnCode, string message, string? localizationKey)
                result = Activator.CreateInstance(resultType, 502, SH.WebHutaoServiceUnAvailable, default) as TResult;
                return true;
            }

            if (resultType.IsConstructedGenericType && resultType.GetGenericTypeDefinition() == typeof(HutaoResponse<>))
            {
                // HutaoResponse<TData>(int returnCode, string message, TData? data, string? localizationKey)
                result = Activator.CreateInstance(resultType, 502, SH.WebHutaoServiceUnAvailable, default, default) as TResult;
                return true;
            }
        }

        return false;
    }

    [SuppressMessage("", "CA1305")]
    private static void ProcessException(StringBuilder builder, Exception exception)
    {
        if (exception is HttpRequestException hre)
        {
            builder
                .AppendLine($"{nameof(HttpRequestException)}: Status Code: {hre.StatusCode} Error: {hre.HttpRequestError}")
                .AppendLine(hre.Message);
        }

        if (exception is IOException ioe)
        {
            builder
                .AppendLine($"{nameof(IOException)}: 0x{ioe.HResult:X8}")
                .AppendLine(ioe.Message);
        }

        if (exception is JsonException je)
        {
            builder
                .AppendLine($"{nameof(JsonException)}: Path: {je.Path} at Line: {je.LineNumber} Position: {je.BytePositionInLine}")
                .AppendLine(je.Message);
        }

        if (exception is HttpContentSerializationException hcse)
        {
            builder
                .AppendLine($"{nameof(HttpContentSerializationException)}:")
                .AppendLine(hcse.Message);
        }

        if (exception is SocketException se)
        {
            builder
                .AppendLine($"{nameof(SocketException)}: Error: {se.SocketErrorCode}")
                .AppendLine(se.Message);
        }

        if (exception.InnerException is { } inner)
        {
            builder.AppendLine(new string('-', 40));
            ProcessException(builder, inner);
        }
    }
}