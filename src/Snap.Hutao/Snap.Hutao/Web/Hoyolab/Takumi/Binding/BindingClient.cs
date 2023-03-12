// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

/// <summary>
/// 绑定客户端
/// </summary>
[HighQuality]
[HttpClient(HttpClientConfigration.Default)]
internal sealed class BindingClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<BindingClient> logger;

    /// <summary>
    /// 构造一个新的用户游戏角色提供器
    /// </summary>
    /// <param name="httpClient">请求器</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="logger">日志器</param>
    public BindingClient(HttpClient httpClient, JsonSerializerOptions options, ILogger<BindingClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 获取用户角色信息
    /// </summary>
    /// <param name="actionTicket">操作凭证</param>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    [ApiInformation(Cookie = CookieType.Ltoken)]
    public async Task<Response<ListWrapper<UserGameRole>>> GetUserGameRolesByActionTicketAsync(string actionTicket, User user, CancellationToken token = default)
    {
        string url = ApiEndpoints.UserGameRolesByActionTicket(actionTicket);

        Response<ListWrapper<UserGameRole>>? resp = await httpClient
            .SetUser(user, CookieType.Ltoken)
            .TryCatchGetFromJsonAsync<Response<ListWrapper<UserGameRole>>>(url, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 获取国际服用户角色信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    [ApiInformation(Cookie = CookieType.Ltoken)]
    public async Task<Response<ListWrapper<UserGameRole>>> GetOsUserGameRolesByCookieAsync(User user, CancellationToken token = default)
    {
        string url = ApiOsEndpoints.UserGameRolesByCookie;

        Response<ListWrapper<UserGameRole>>? resp = await httpClient
            .SetUser(user, CookieType.Ltoken)
            .TryCatchGetFromJsonAsync<Response<ListWrapper<UserGameRole>>>(url, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
