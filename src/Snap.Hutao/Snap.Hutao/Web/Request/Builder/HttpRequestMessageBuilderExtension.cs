// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.Response;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;

namespace Snap.Hutao.Web.Request.Builder;

internal static class HttpRequestMessageBuilderExtension
{
    private const string RequestErrorMessage = "请求异常已忽略: {Uri}";

    internal static void Resurrect(this HttpRequestMessageBuilder builder)
    {
        builder.HttpRequestMessage.Resurrect();
    }

    internal static async ValueTask<TResult?> SendAsync<TResult>(this HttpRequestMessageBuilder builder, HttpClient httpClient, ILogger logger, CancellationToken token)
        where TResult : class
    {
        try
        {
            using (builder.HttpRequestMessage)
            {
                using (HttpResponseMessage message = await httpClient.SendAsync(builder.HttpRequestMessage, token).ConfigureAwait(false))
                {
                    message.EnsureSuccessStatusCode();
                    return await builder.HttpContentSerializer.DeserializeAsync<TResult>(message.Content, token).ConfigureAwait(false);
                }
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.HttpRequestMessage.RequestUri);

            if (ex.StatusCode is HttpStatusCode.BadGateway)
            {
                Type resultType = typeof(TResult);

                if (resultType == typeof(HutaoResponse))
                {
                    return Activator.CreateInstance(resultType, 502, SH.WebHutaoServiceUnAvailable, default) as TResult;
                }

                if (resultType.IsConstructedGenericType && resultType.GetGenericTypeDefinition() == typeof(HutaoResponse<>))
                {
                    return Activator.CreateInstance(resultType, 502, SH.WebHutaoServiceUnAvailable, default, default) as TResult;
                }
            }

            return default;
        }
        catch (IOException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.HttpRequestMessage.RequestUri);
            return default;
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.HttpRequestMessage.RequestUri);
            return default;
        }
        catch (HttpContentSerializationException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.HttpRequestMessage.RequestUri);
            return default;
        }
        catch (SocketException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.HttpRequestMessage.RequestUri);
            return default;
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
            logger.LogWarning(ex, RequestErrorMessage, builder.HttpRequestMessage.RequestUri);
        }
        catch (IOException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.HttpRequestMessage.RequestUri);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.HttpRequestMessage.RequestUri);
        }
        catch (HttpContentSerializationException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.HttpRequestMessage.RequestUri);
        }
        catch (SocketException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.HttpRequestMessage.RequestUri);
        }
    }
}