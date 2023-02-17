// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

/// <summary>
/// 公告客户端
/// </summary>
[HttpClient(HttpClientConfigration.Default)]
internal sealed class AnnouncementClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger<AnnouncementClient> logger;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    /// <summary>
    /// 构造一个新的公告客户端
    /// </summary>
    /// <param name="httpClient">客户端</param>
    /// <param name="logger">日志器</param>
    /// <param name="jsonSerializerOptions">json序列化选项</param>
    public AnnouncementClient(HttpClient httpClient, ILogger<AnnouncementClient> logger, JsonSerializerOptions jsonSerializerOptions)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        this.jsonSerializerOptions = jsonSerializerOptions;
    }

    /// <summary>
    /// 异步获取公告列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>公告列表</returns>
    public async Task<Response<AnnouncementWrapper>> GetAnnouncementsAsync(CancellationToken cancellationToken = default)
    {
        Response<AnnouncementWrapper>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<AnnouncementWrapper>>(ApiEndpoints.AnnList, jsonSerializerOptions, logger, cancellationToken)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取公告内容列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>公告内容列表</returns>
    public async Task<Response<ListWrapper<AnnouncementContent>>> GetAnnouncementContentsAsync(CancellationToken cancellationToken = default)
    {
        Response<ListWrapper<AnnouncementContent>>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<ListWrapper<AnnouncementContent>>>(ApiEndpoints.AnnContent, jsonSerializerOptions, logger, cancellationToken)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
