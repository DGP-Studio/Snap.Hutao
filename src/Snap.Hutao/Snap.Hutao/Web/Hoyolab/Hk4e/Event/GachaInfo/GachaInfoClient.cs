// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
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
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<GachaInfoClient> logger;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 获取记录页面
    /// </summary>
    /// <param name="options">查询</param>
    /// <param name="token">取消令牌</param>
    /// <returns>单个祈愿记录页面</returns>
    public async ValueTask<Response<GachaLogPage>> GetGachaLogPageAsync(GachaLogQueryOptions options, CancellationToken token = default)
    {
        string query = options.ToQueryString();

        string url = options.IsOversea
            ? ApiOsEndpoints.GachaInfoGetGachaLog(query)
            : ApiEndpoints.GachaInfoGetGachaLog(query);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(url)
            .Get();

        Response<GachaLogPage>? resp = await builder
            .TryCatchSendAsync<Response<GachaLogPage>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}