// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Logging;
using Snap.Hutao.Model.Binding;
using Snap.Hutao.Web.Request;
using System.Net.Http;
using System.Net.Http.Json;

namespace Snap.Hutao.Web.Hoyolab;

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
    }

    /// <summary>
    /// 设置用户的Cookie
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="user">用户</param>
    /// <returns>客户端</returns>
    internal static HttpClient SetUser(this HttpClient httpClient, User user)
    {
        httpClient.DefaultRequestHeaders.Set("Cookie", user.Cookie?.ToString() ?? string.Empty);
        return httpClient;
    }

    /// <summary>
    /// 设置Referer
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="referer">用户</param>
    /// <returns>客户端</returns>
    internal static HttpClient SetReferer(this HttpClient httpClient, string referer)
    {
        httpClient.DefaultRequestHeaders.Set("Referer", referer);
        return httpClient;
    }
}
