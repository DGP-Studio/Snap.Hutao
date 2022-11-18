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
    /// 使用动态密钥
    /// </summary>
    /// <param name="client">客户端</param>
    /// <param name="version">版本</param>
    /// <param name="salt">盐</param>
    /// <param name="includeChars">包括大小写英文字母</param>
    /// <returns>同一个客户端</returns>
    public static HttpClient UseDynamicSecret(this HttpClient client, DynamicSecretVersion version, SaltType salt, bool includeChars)
    {
        client.DefaultRequestHeaders.Set("DS-Option", $"{version}|{salt}|{includeChars}");
        return client;
    }

    /// <summary>
    /// 使用一代动态密钥执行 GET/POST 操作
    /// </summary>
    /// <param name="httpClient">请求器</param>
    /// <param name="type">SALT 类型</param>
    /// <returns>响应</returns>
    [Obsolete]
    public static HttpClient UsingDynamicSecret1(this HttpClient httpClient, SaltType type)
    {
        httpClient.DefaultRequestHeaders.Set("DS", DynamicSecretProvider.Create(type));
        return httpClient;
    }

    /// <summary>
    /// 使用二代动态密钥执行 GET/POST 操作
    /// </summary>
    /// <param name="httpClient">请求器</param>
    /// <param name="type">SALT 类型</param>
    /// <param name="options">选项</param>
    /// <param name="url">地址</param>
    /// <returns>响应</returns>
    [Obsolete]
    public static IDynamicSecretHttpClient UsingDynamicSecret2(this HttpClient httpClient, SaltType type, JsonSerializerOptions options, string url)
    {
        return new DynamicSecretHttpClient(httpClient, type, options, url);
    }

    /// <summary>
    /// 使用二代动态密钥执行 GET/POST 操作
    /// </summary>
    /// <typeparam name="TValue">请求数据的类型</typeparam>
    /// <param name="httpClient">请求器</param>
    /// <param name="type">SALT 类型</param>
    /// <param name="options">选项</param>
    /// <param name="url">地址</param>
    /// <param name="data">post数据</param>
    /// <returns>响应</returns>
    public static IDynamicSecretHttpClient<TValue> UsingDynamicSecret2<TValue>(this HttpClient httpClient, SaltType type, JsonSerializerOptions options, string url, TValue data)
        where TValue : class
    {
        return new DynamicSecretHttpClient<TValue>(httpClient, type, options, url, data);
    }
}
