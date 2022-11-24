// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Logging;
using Snap.Hutao.Extension;
using Snap.Hutao.Web.Request;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;

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
        catch (SocketException ex)
        {
            logger.LogWarning(EventIds.HttpException, ex, "请求异常已忽略");
            return null;
        }
    }

    /// <inheritdoc cref="HttpClientJsonExtensions.PostAsJsonAsync{TValue}(HttpClient, string?, TValue, JsonSerializerOptions?, CancellationToken)"/>
    internal static async Task<TResult?> TryCatchPostAsJsonAsync<TValue, TResult>(this HttpClient httpClient, string requestUri, TValue value, JsonSerializerOptions options, CancellationToken token = default)
        where TResult : class
    {
        try
        {
            HttpResponseMessage message = await httpClient.PostAsJsonAsync(requestUri, value, options, token).ConfigureAwait(false);
            return await message.Content.ReadFromJsonAsync<TResult>(options, token).ConfigureAwait(false);
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (SocketException)
        {
            return null;
        }
    }

    /// <summary>
    /// 设置用户的 Cookie
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="user">实体用户</param>
    /// <param name="cookie">Cookie类型</param>
    /// <returns>客户端</returns>
    internal static HttpClient SetUser(this HttpClient httpClient, Model.Entity.User user, CookieType cookie)
    {
        httpClient.DefaultRequestHeaders.Remove("Cookie");
        StringBuilder stringBuilder = new();

        if ((cookie & CookieType.CookieToken) == CookieType.CookieToken)
        {
            stringBuilder.Append(user.CookieToken).AppendIf(user.CookieToken != null, ';');
        }

        if ((cookie & CookieType.Ltoken) == CookieType.Ltoken)
        {
            stringBuilder.Append(user.Ltoken).AppendIf(user.Ltoken != null, ';');
        }

        if ((cookie & CookieType.Stoken) == CookieType.Stoken)
        {
            stringBuilder.Append(user.Stoken).AppendIf(user.Stoken != null, ';');
        }

        if ((cookie & CookieType.Mid) == CookieType.Mid)
        {
            stringBuilder.Append("mid=").Append(user.Mid).Append(';');
        }

        httpClient.DefaultRequestHeaders.Set("Cookie", stringBuilder.ToString());
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

    /// <summary>
    /// 设置验证流水号
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="challenge">验证流水号</param>
    /// <returns>客户端</returns>
    internal static HttpClient SetXrpcChallenge(this HttpClient httpClient, string challenge)
    {
        httpClient.DefaultRequestHeaders.Set("x-rpc-challenge", challenge);
        return httpClient;
    }

    /// <summary>
    /// 设置头
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>客户端</returns>
    internal static HttpClient SetHeader(this HttpClient httpClient, string key, string value)
    {
        httpClient.DefaultRequestHeaders.Set(key, value);
        return httpClient;
    }
}
