// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Logging;
using Snap.Hutao.Extension;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;

namespace Snap.Hutao.Web;

/// <summary>
/// <see cref="HttpClient"/> 扩展
/// </summary>
internal static class HttpClientExtensions
{
    /// <inheritdoc cref="HttpClientJsonExtensions.GetFromJsonAsync{TValue}(HttpClient, string?, JsonSerializerOptions?, CancellationToken)"/>
    internal static async Task<T?> TryCatchGetFromJsonAsync<T>(this HttpClient httpClient, string requestUri, JsonSerializerOptions options, ILogger logger, CancellationToken token = default)
        where T : class
    {
        try
        {
            return await httpClient.GetFromJsonAsync<T>(requestUri, options, token).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(EventIds.HttpException, ex, "请求异常已忽略");
            return null;
        }
        catch (IOException ex)
        {
            logger.LogWarning(EventIds.HttpException, ex, "请求异常已忽略");
            return null;
        }
        catch (JsonException ex)
        {
            logger.LogWarning(EventIds.HttpException, ex, "请求异常已忽略");
            return null;
        }
        catch (SocketException ex)
        {
            logger.LogWarning(EventIds.HttpException, ex, "请求异常已忽略");
            return null;
        }
    }

    /// <inheritdoc cref="HttpClientJsonExtensions.PostAsJsonAsync{TValue}(HttpClient, string?, TValue, JsonSerializerOptions?, CancellationToken)"/>
    internal static async Task<TResult?> TryCatchPostAsJsonAsync<TValue, TResult>(this HttpClient httpClient, string requestUri, TValue value, JsonSerializerOptions options, ILogger logger, CancellationToken token = default)
        where TResult : class
    {
        try
        {
            HttpResponseMessage message = await httpClient.PostAsJsonAsync(requestUri, value, options, token).ConfigureAwait(false);
            return await message.Content.ReadFromJsonAsync<TResult>(options, token).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(EventIds.HttpException, ex, "请求异常已忽略");
            return null;
        }
        catch (IOException ex)
        {
            logger.LogWarning(EventIds.HttpException, ex, "请求异常已忽略");
            return null;
        }
        catch (JsonException ex)
        {
            logger.LogWarning(EventIds.HttpException, ex, "请求异常已忽略");
            return null;
        }
        catch (SocketException ex)
        {
            logger.LogWarning(EventIds.HttpException, ex, "请求异常已忽略");
            return null;
        }
    }

    /// <inheritdoc cref="HttpClientJsonExtensions.PostAsJsonAsync{TValue}(HttpClient, string?, TValue, JsonSerializerOptions?, CancellationToken)"/>
    internal static async Task<TResult?> TryCatchPostAsJsonAsync<TValue, TResult>(this HttpClient httpClient, string requestUri, TValue value, CancellationToken token = default)
        where TResult : class
    {
        try
        {
            HttpResponseMessage message = await httpClient.PostAsJsonAsync(requestUri, value, token).ConfigureAwait(false);
            return await message.Content.ReadFromJsonAsync<TResult>(cancellationToken: token).ConfigureAwait(false);
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
        catch (SocketException)
        {
            return null;
        }
    }
}
