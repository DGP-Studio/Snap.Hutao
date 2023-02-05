// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

/// <summary>
/// 祈愿记录客户端
/// </summary>
[HttpClient(HttpClientConfigration.Default)]
internal class GachaInfoClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<GachaInfoClient> logger;

    /// <summary>
    /// 构造一个新的祈愿记录客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="logger">日志器</param>
    public GachaInfoClient(HttpClient httpClient, JsonSerializerOptions options, ILogger<GachaInfoClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 获取记录页面
    /// </summary>
    /// <param name="config">查询</param>
    /// <param name="token">取消令牌</param>
    /// <returns>单个祈愿记录页面</returns>
    public async Task<Response<GachaLogPage>> GetGachaLogPageAsync(GachaLogConfigration config, CancellationToken token = default)
    {
        string query = config.AsQuery();

        // TODO: fix oversea behavior
        string url = query.Contains("hoyoverse.com")
            ? ApiOsEndpoints.GachaInfoGetGachaLog(query)
            : ApiEndpoints.GachaInfoGetGachaLog(query);

        Response<GachaLogPage>? response = await httpClient
            .TryCatchGetFromJsonAsync<Response<GachaLogPage>>(url, options, logger, token)
            .ConfigureAwait(false);
        return Response.Response.DefaultIfNull(response);
    }
}