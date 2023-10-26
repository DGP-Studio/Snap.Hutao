// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;
using System.Net.Http;
using System.Net.Sockets;

namespace Snap.Hutao.Web.Request.Builder;

internal static class HttpRequestMessageBuilderExtension
{
    private const string RequestErrorMessage = "请求异常已忽略";

    internal static async ValueTask<TResult?> TryCatchSendAsync<TResult>(this HttpRequestMessageBuilder builder, HttpClient httpClient, ILogger logger, CancellationToken token)
    {
        try
        {
            HttpResponseMessage message = await httpClient.SendAsync(builder.HttpRequestMessage, token).ConfigureAwait(false);
            return await builder.HttpContentSerializer.DeserializeAsync<TResult>(message.Content, token).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage);
            return default;
        }
        catch (IOException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage);
            return default;
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage);
            return default;
        }
        catch (HttpContentSerializationException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage);
            return default;
        }
        catch (SocketException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage);
            return default;
        }
    }

    internal static async ValueTask TryCatchSendAsync(this HttpRequestMessageBuilder builder, HttpClient httpClient, ILogger logger, CancellationToken token)
    {
        try
        {
            HttpResponseMessage message = await httpClient.SendAsync(builder.HttpRequestMessage, token).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage);
        }
        catch (IOException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage);
        }
        catch (HttpContentSerializationException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage);
        }
        catch (SocketException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage);
        }
    }
}