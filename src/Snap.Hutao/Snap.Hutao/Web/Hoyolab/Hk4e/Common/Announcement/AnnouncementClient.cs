// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using Snap.Hutao.Web.Response;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

/// <summary>
/// 公告客户端
/// </summary>
[Injection(InjectAs.Transient)]
internal class AnnouncementClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    /// <summary>
    /// 构造一个新的公告客户端
    /// </summary>
    /// <param name="httpClient">客户端</param>
    /// <param name="jsonSerializerOptions">json序列化选项</param>
    public AnnouncementClient(HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions)
    {
        this.httpClient = httpClient;
        this.jsonSerializerOptions = jsonSerializerOptions;
    }

    /// <summary>
    /// 异步获取公告列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>公告列表</returns>
    public async Task<AnnouncementWrapper?> GetAnnouncementsAsync(CancellationToken cancellationToken = default)
    {
        Response<AnnouncementWrapper>? resp = await httpClient
            .GetFromJsonAsync<Response<AnnouncementWrapper>>(ApiEndpoints.AnnList, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 异步获取公告内容列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>公告内容列表</returns>
    public async Task<List<AnnouncementContent>> GetAnnouncementContentsAsync(CancellationToken cancellationToken = default)
    {
        // Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Response<ListWrapper<AnnouncementContent>>? resp = await httpClient
            .GetFromJsonAsync<Response<ListWrapper<AnnouncementContent>>>(ApiEndpoints.AnnContent, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data?.List);
    }
}
