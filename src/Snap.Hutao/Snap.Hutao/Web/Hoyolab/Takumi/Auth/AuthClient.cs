// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Auth;

/// <summary>
/// 授权客户端
/// </summary>
[HighQuality]
[UseDynamicSecret]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class AuthClient
{
    private readonly JsonSerializerOptions options;
    private readonly ILogger<BindingClient> logger;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 异步获取操作凭证
    /// </summary>
    /// <param name="action">操作</param>
    /// <param name="user">用户</param>
    /// <returns>操作凭证</returns>
    [ApiInformation(Cookie = CookieType.SToken, Salt = SaltType.K2)]
    public async ValueTask<Response<ActionTicketWrapper>> GetActionTicketBySTokenAsync(string action, User user)
    {
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);
        string url = ApiEndpoints.AuthActionTicket(action, user.SToken?[Cookie.STOKEN] ?? string.Empty, user.Aid);

        Response<ActionTicketWrapper>? resp = await httpClient
            .SetUser(user, CookieType.SToken)
            .UseDynamicSecret(DynamicSecretVersion.Gen1, SaltType.K2, true)
            .TryCatchGetFromJsonAsync<Response<ActionTicketWrapper>>(url, options, logger)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 获取 MultiToken
    /// </summary>
    /// <param name="cookie">login cookie</param>
    /// <param name="isOversea">是否为国际服</param>
    /// <param name="token">取消令牌</param>
    /// <returns>包含token的字典</returns>
    public async ValueTask<Response<ListWrapper<NameToken>>> GetMultiTokenByLoginTicketAsync(Cookie cookie, bool isOversea, CancellationToken token = default)
    {
        string loginTicket = cookie[Cookie.LOGIN_TICKET];
        string loginUid = cookie[Cookie.LOGIN_UID];

        string url = isOversea
            ? ApiOsEndpoints.AuthMultiToken(loginTicket, loginUid)
            : ApiEndpoints.AuthMultiToken(loginTicket, loginUid);

        Response<ListWrapper<NameToken>>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<ListWrapper<NameToken>>>(url, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}