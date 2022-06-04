// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Request;
using Snap.Hutao.Web.Response;
using System.Collections.Generic;
using System.Text;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Common.Announcement;

/// <summary>
/// 公告提供器
/// </summary>
[Injection(InjectAs.Transient)]
internal class AnnouncementProvider
{
    private readonly Requester requester;

    /// <summary>
    /// 构造一个新的公告提供器
    /// </summary>
    /// <param name="requester">请求器</param>
    public AnnouncementProvider(Requester requester)
    {
        this.requester = requester;
    }

    /// <summary>
    /// 异步获取公告列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>公告列表</returns>
    public async Task<AnnouncementWrapper?> GetAnnouncementsAsync(CancellationToken cancellationToken = default)
    {
        Response<AnnouncementWrapper>? resp = await requester
            .Reset()
            .GetAsync<AnnouncementWrapper>(ApiEndpoints.AnnList, cancellationToken)
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
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        Response<ListWrapper<AnnouncementContent>>? resp = await requester
            .Reset()
            .AddHeader(RequestHeaders.Accept, RequestOptions.Json)
            .GetAsync<ListWrapper<AnnouncementContent>>(ApiEndpoints.AnnContent, cancellationToken)
            .ConfigureAwait(false);

        return resp?.Data?.List ?? new();
    }
}
