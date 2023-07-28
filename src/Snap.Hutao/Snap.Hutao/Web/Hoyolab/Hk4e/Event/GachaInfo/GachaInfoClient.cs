// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

/// <summary>
/// 祈愿记录客户端
/// </summary>
[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class GachaInfoClient
{
    private readonly ILogger<GachaInfoClient> logger;
    private readonly JsonSerializerOptions options;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 获取记录页面
    /// </summary>
    /// <param name="options">查询</param>
    /// <param name="token">取消令牌</param>
    /// <returns>单个祈愿记录页面</returns>
    public async Task<Response<GachaLogPage>> GetGachaLogPageAsync(GachaLogQueryOptions options, CancellationToken token = default)
    {
        string query = options.ToQueryString();

        string url = options.IsOversea
            ? ApiOsEndpoints.GachaInfoGetGachaLog(query)
            : ApiEndpoints.GachaInfoGetGachaLog(query);

        Response<GachaLogPage>? response = await httpClient
            .TryCatchGetFromJsonAsync<Response<GachaLogPage>>(url, this.options, logger, token)
            .ConfigureAwait(false);
        return Response.Response.DefaultIfNull(response);
    }
}