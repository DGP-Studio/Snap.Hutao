// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

/// <summary>
/// 绑定客户端
/// </summary>
[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class BindingClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<BindingClient> logger;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 异步获取用户角色信息
    /// 自动判断是否为国际服
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    public async ValueTask<Response<ListWrapper<UserGameRole>>> GetUserGameRolesOverseaAwareAsync(User user, CancellationToken token = default)
    {
        if (user.IsOversea)
        {
            return await GetOverseaUserGameRolesByCookieAsync(user, token).ConfigureAwait(false);
        }
        else
        {
            Response<ActionTicketWrapper> actionTicketResponse = await serviceProvider
                .GetRequiredService<AuthClient>()
                .GetActionTicketBySTokenAsync("game_role", user, token)
                .ConfigureAwait(false);

            if (actionTicketResponse.IsOk())
            {
                string actionTicket = actionTicketResponse.Data.Ticket;
                return await GetUserGameRolesByActionTicketAsync(actionTicket, user, token).ConfigureAwait(false);
            }
            else
            {
                return Response.Response.DefaultIfNull<ListWrapper<UserGameRole>, ActionTicketWrapper>(actionTicketResponse);
            }
        }
    }

    /// <summary>
    /// 异步获取用户角色信息
    /// </summary>
    /// <param name="actionTicket">操作凭证</param>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    [ApiInformation(Cookie = CookieType.LToken)]
    public async ValueTask<Response<ListWrapper<UserGameRole>>> GetUserGameRolesByActionTicketAsync(string actionTicket, User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.UserGameRolesByActionTicket(actionTicket))
            .SetUserCookie(user, CookieType.LToken)
            .Get();

        Response<ListWrapper<UserGameRole>>? resp = await builder
            .TryCatchSendAsync<Response<ListWrapper<UserGameRole>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取国际服用户角色信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    [ApiInformation(Cookie = CookieType.LToken)]
    public async ValueTask<Response<ListWrapper<UserGameRole>>> GetOverseaUserGameRolesByCookieAsync(User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.UserGameRolesByCookie)
            .SetUserCookie(user, CookieType.LToken)
            .Get();

        Response<ListWrapper<UserGameRole>>? resp = await builder
            .TryCatchSendAsync<Response<ListWrapper<UserGameRole>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
