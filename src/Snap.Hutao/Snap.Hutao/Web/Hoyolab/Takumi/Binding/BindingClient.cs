// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
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
    private readonly IServiceProvider serviceProvider;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<BindingClient> logger;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 异步获取用户角色信息
    /// 自动判断是否为国际服
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    public async Task<Response<ListWrapper<UserGameRole>>> GetUserGameRolesOverseaAwareAsync(User user, CancellationToken token = default)
    {
        if (user.IsOversea)
        {
            return await GetOverseaUserGameRolesByCookieAsync(user, token).ConfigureAwait(false);
        }
        else
        {
            Response<ActionTicketWrapper> actionTicketResponse = await serviceProvider
                .GetRequiredService<AuthClient>()
                .GetActionTicketBySTokenAsync("game_role", user)
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
    public async Task<Response<ListWrapper<UserGameRole>>> GetUserGameRolesByActionTicketAsync(string actionTicket, User user, CancellationToken token = default)
    {
        string url = ApiEndpoints.UserGameRolesByActionTicket(actionTicket);

        Response<ListWrapper<UserGameRole>>? resp = await httpClient
            .SetUser(user, CookieType.LToken)
            .TryCatchGetFromJsonAsync<Response<ListWrapper<UserGameRole>>>(url, options, logger, token)
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
    public async Task<Response<ListWrapper<UserGameRole>>> GetOverseaUserGameRolesByCookieAsync(User user, CancellationToken token = default)
    {
        Response<ListWrapper<UserGameRole>>? resp = await httpClient
            .SetUser(user, CookieType.LToken)
            .TryCatchGetFromJsonAsync<Response<ListWrapper<UserGameRole>>>(ApiOsEndpoints.UserGameRolesByCookie, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
