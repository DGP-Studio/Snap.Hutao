// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Logging;
using Snap.Hutao.Web.Request;
using System.Net.Http;
using System.Net.Http.Json;

namespace Snap.Hutao.Web.Hoyolab.DynamicSecret.Http;

/// <summary>
/// 使用动态密钥2的Http客户端默认实现
/// </summary>
/// <typeparam name="TValue">请求提交的数据的的格式</typeparam>
internal class DynamicSecretHttpClient : IDynamicSecretHttpClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly string url;

    /// <summary>
    /// 构造一个新的使用动态密钥2的Http客户端默认实现的实例
    /// </summary>
    /// <param name="httpClient">请求使用的客户端</param>
    /// <param name="saltType">SALT 类型</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="url">url</param>
    public DynamicSecretHttpClient(HttpClient httpClient, SaltType saltType, JsonSerializerOptions options, string url)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.url = url;

        httpClient.DefaultRequestHeaders.Set("DS", DynamicSecretProvider2.Create(saltType, options, url, null));
    }

    /// <inheritdoc/>
    public Task<TValue?> GetFromJsonAsync<TValue>(CancellationToken token)
    {
        return httpClient.GetFromJsonAsync<TValue>(url, options, token);
    }

    /// <inheritdoc/>
    public async Task<T?> TryCatchGetFromJsonAsync<T>(CancellationToken token = default)
        where T : class
    {
        try
        {
            return await httpClient.GetFromJsonAsync<T>(url, options, token).ConfigureAwait(false);
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<T?> TryCatchGetFromJsonAsync<T>(ILogger logger, CancellationToken token = default)
        where T : class
    {
        try
        {
            return await httpClient.GetFromJsonAsync<T>(url, options, token).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(EventIds.HttpException, ex, "请求异常已忽略");
            return null;
        }
    }
}

/// <summary>
/// 使用动态密钥2的Http客户端默认实现
/// </summary>
/// <typeparam name="TValue">请求提交的数据的的格式</typeparam>
[SuppressMessage("", "SA1402")]
internal class DynamicSecretHttpClient<TValue> : IDynamicSecretHttpClient<TValue>
    where TValue : class
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly string url;
    private readonly TValue data;

    /// <summary>
    /// 构造一个新的使用动态密钥2的Http客户端默认实现的实例
    /// </summary>
    /// <param name="httpClient">请求使用的客户端</param>
    /// <param name="saltType">SALT 类型</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="url">url</param>
    /// <param name="data">请求的数据</param>
    public DynamicSecretHttpClient(HttpClient httpClient, SaltType saltType, JsonSerializerOptions options, string url, TValue data)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.url = url;
        this.data = data;

        httpClient.DefaultRequestHeaders.Set("DS", DynamicSecretProvider2.Create(saltType, options, url, data));
    }

    /// <inheritdoc/>
    public Task<HttpResponseMessage> PostAsJsonAsync(CancellationToken token)
    {
        return httpClient.PostAsJsonAsync(url, data, options, token);
    }

    /// <inheritdoc/>
    public Task<TResult?> TryCatchPostAsJsonAsync<TResult>(ILogger logger, CancellationToken token = default(CancellationToken))
        where TResult : class
    {
        return httpClient.TryCatchPostAsJsonAsync<TValue, TResult>(url, data, options, logger, token);
    }
}