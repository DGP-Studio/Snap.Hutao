// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;

namespace Snap.Hutao.Core.Json;

/// <summary>
/// Http Json 处理
/// </summary>
public class HttpJson
{
    private readonly Json json;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 初始化一个新的 Http Json 处理 实例
    /// </summary>
    /// <param name="json">Json 处理器</param>
    /// <param name="httpClient">http 客户端</param>
    public HttpJson(Json json, HttpClient httpClient)
    {
        this.json = json;
        this.httpClient = httpClient;
    }

    /// <summary>
    /// 从网站上下载json并转换为对象
    /// </summary>
    /// <typeparam name="T">对象的类型</typeparam>
    /// <param name="url">链接</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>Json字符串中的反序列化对象, 如果反序列化失败会抛出异常</returns>
    public async Task<T?> FromWebsiteAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        string response = await httpClient.GetStringAsync(url, cancellationToken);
        return json.ToObject<T>(response);
    }
}