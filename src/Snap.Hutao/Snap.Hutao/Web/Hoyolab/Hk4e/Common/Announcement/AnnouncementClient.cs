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
    /// <param name="languageCode">语言代码</param>
    /// <param name="region">服务器</param>
    /// <param name="token">取消令牌</param>
    /// <returns>公告列表</returns>
    public async ValueTask<Response<AnnouncementWrapper>> GetAnnouncementsAsync(string languageCode, RegionType region, CancellationToken token = default)
    {
        string annListUrl = region switch
        {
            RegionType.CN_GF01 => ApiEndpoints.AnnList(languageCode, region.ToString().ToLowerInvariant()),
            RegionType.CN_QD01 => ApiEndpoints.AnnList(languageCode, region.ToString().ToLowerInvariant()),
            RegionType.OS_USA => ApiOsEndpoints.AnnList(languageCode, region.ToString().ToLowerInvariant()),
            RegionType.OS_EURO => ApiOsEndpoints.AnnList(languageCode, region.ToString().ToLowerInvariant()),
            RegionType.OS_ASIA => ApiOsEndpoints.AnnList(languageCode, region.ToString().ToLowerInvariant()),
            RegionType.OS_CHT => ApiOsEndpoints.AnnList(languageCode, region.ToString().ToLowerInvariant()),
            _ => ApiEndpoints.AnnList(languageCode, RegionType.CN_GF01.ToString().ToLowerInvariant()),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(annListUrl)
            .Get();

        Response<AnnouncementWrapper>? resp = await builder
            .TryCatchSendAsync<Response<AnnouncementWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取公告内容列表
    /// </summary>
    /// <param name="languageCode">语言代码</param>
    /// <param name="region">服务器</param>
    /// <param name="token">取消令牌</param>
    /// <returns>公告内容列表</returns>
    public async ValueTask<Response<ListWrapper<AnnouncementContent>>> GetAnnouncementContentsAsync(string languageCode, RegionType region, CancellationToken token = default)
    {
        string annContentUrl = region switch
        {
            RegionType.CN_GF01 => ApiEndpoints.AnnContent(languageCode, region.ToString().ToLowerInvariant()),
            RegionType.CN_QD01 => ApiEndpoints.AnnContent(languageCode, region.ToString().ToLowerInvariant()),
            RegionType.OS_USA => ApiOsEndpoints.AnnContent(languageCode, region.ToString().ToLowerInvariant()),
            RegionType.OS_EURO => ApiOsEndpoints.AnnContent(languageCode, region.ToString().ToLowerInvariant()),
            RegionType.OS_ASIA => ApiOsEndpoints.AnnContent(languageCode, region.ToString().ToLowerInvariant()),
            RegionType.OS_CHT => ApiOsEndpoints.AnnContent(languageCode, region.ToString().ToLowerInvariant()),
            _ => ApiEndpoints.AnnContent(languageCode, RegionType.CN_GF01.ToString().ToLowerInvariant()),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(annContentUrl)
            .Get();

        Response<ListWrapper<AnnouncementContent>>? resp = await builder
            .TryCatchSendAsync<Response<ListWrapper<AnnouncementContent>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
