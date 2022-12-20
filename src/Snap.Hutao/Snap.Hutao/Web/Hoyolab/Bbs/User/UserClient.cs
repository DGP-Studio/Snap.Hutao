// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 用户信息客户端
/// </summary>
[HttpClient(HttpClientConfigration.XRpc)]
internal class UserClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<UserClient> logger;

    /// <summary>
    /// 构造一个新的用户信息客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="logger">日志器</param>
    public UserClient(HttpClient httpClient, JsonSerializerOptions options, ILogger<UserClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 获取当前用户详细信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>详细信息</returns>
    [ApiInformation(Cookie = CookieType.Ltoken)]
    public async Task<UserInfo?> GetUserFullInfoAsync(Model.Entity.User user, CancellationToken token = default)
    {
        Response<UserFullInfoWrapper>? resp = await httpClient
            .SetUser(user, CookieType.Ltoken)
            .SetReferer(ApiEndpoints.BbsReferer) // Otherwise HTTP 403
            .TryCatchGetFromJsonAsync<Response<UserFullInfoWrapper>>(ApiEndpoints.UserFullInfo, options, logger, token)
            .ConfigureAwait(false);

        return resp?.Data?.UserInfo;
    }

    /// <summary>
    /// 获取当前用户详细信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>详细信息</returns>
    [ApiInformation(Cookie = CookieType.Ltoken)]
    public async Task<UserInfo?> GetUserFullInfoByUidAsync(Model.Entity.User user, CancellationToken token = default)
    {
        Response<UserFullInfoWrapper>? resp = await httpClient

            // .SetUser(user, CookieType.Cookie)
            .SetReferer(ApiEndpoints.BbsReferer) // Otherwise HTTP 403
            .TryCatchGetFromJsonAsync<Response<UserFullInfoWrapper>>(ApiEndpoints.UserFullInfoQuery(user.Aid!), options, logger, token)
            .ConfigureAwait(false);

        return resp?.Data?.UserInfo;
    }
}