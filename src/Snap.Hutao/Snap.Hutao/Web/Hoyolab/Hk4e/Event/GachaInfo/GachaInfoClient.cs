// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Response;
using System.Net.Http;
using System.Net.Http.Json;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

/// <summary>
/// 祈愿记录客户端
/// </summary>
[HttpClient(HttpClientConfigration.Default)]
internal class GachaInfoClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;

    /// <summary>
    /// 构造一个新的祈愿记录客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">Json序列化选项</param>
    public GachaInfoClient(HttpClient httpClient, JsonSerializerOptions options)
    {
        this.httpClient = httpClient;
        this.options = options;
    }

    /// <summary>
    /// 获取记录页面
    /// </summary>
    /// <param name="config">查询</param>
    /// <param name="token">取消令牌</param>
    /// <returns>单个祈愿记录页面</returns>
    public Task<Response<GachaLogPage>?> GetGachaLogPageAsync(GachaLogConfigration config, CancellationToken token = default)
    {
        string query = config.AsQuery();
        string url = query.Contains("hoyoverse.com")
            ? ApiOsEndpoints.GachaInfoGetGachaLog(query)
            : ApiEndpoints.GachaInfoGetGachaLog(query);

        return httpClient.GetFromJsonAsync<Response<GachaLogPage>>(url, options, token);
    }
}