// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

/// <summary>
/// 公告客户端
/// </summary>
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class AnnouncementClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<AnnouncementClient> logger;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 异步获取公告列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>公告列表</returns>
    public async ValueTask<Response<AnnouncementWrapper>> GetAnnouncementsAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.AnnList)
            .Get();

        Response<AnnouncementWrapper>? resp = await builder
            .TryCatchSendAsync<Response<AnnouncementWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取公告内容列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>公告内容列表</returns>
    public async ValueTask<Response<ListWrapper<AnnouncementContent>>> GetAnnouncementContentsAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.AnnContent)
            .Get();

        Response<ListWrapper<AnnouncementContent>>? resp = await builder
            .TryCatchSendAsync<Response<ListWrapper<AnnouncementContent>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
