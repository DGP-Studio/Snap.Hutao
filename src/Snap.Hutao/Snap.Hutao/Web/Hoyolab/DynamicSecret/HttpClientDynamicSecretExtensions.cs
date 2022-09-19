// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.DynamicSecret.Http;
using Snap.Hutao.Web.Request;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.DynamicSecret;

/// <summary>
/// 动态密钥扩展
/// </summary>
internal static class HttpClientDynamicSecretExtensions
{
    /// <summary>
    /// 使用一代动态密钥执行 GET 操作
    /// </summary>
    /// <param name="httpClient">请求器</param>
    /// <returns>响应</returns>
    public static HttpClient UsingDynamicSecret(this HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Set("DS", DynamicSecretProvider.Create());
        return httpClient;
    }

    /// <summary>
    /// 使用二代动态密钥执行 GET 操作
    /// </summary>
    /// <param name="httpClient">请求器</param>
    /// <param name="options">选项</param>
    /// <param name="url">地址</param>
    /// <returns>响应</returns>
    public static IDynamicSecretHttpClient UsingDynamicSecret(this HttpClient httpClient, JsonSerializerOptions options, string url)
    {
        return new DynamicSecretHttpClient(httpClient, options, url);
    }

    /// <summary>
    /// 使用二代动态密钥执行 GET 操作
    /// </summary>
    /// <typeparam name="TValue">请求数据的类型</typeparam>
    /// <param name="httpClient">请求器</param>
    /// <param name="options">选项</param>
    /// <param name="url">地址</param>
    /// <param name="data">post数据</param>
    /// <returns>响应</returns>
    public static IDynamicSecretHttpClient<TValue> UsingDynamicSecret<TValue>(this HttpClient httpClient, JsonSerializerOptions options, string url, TValue data)
        where TValue : class
    {
        return new DynamicSecretHttpClient<TValue>(httpClient, options, url, data);
    }
}