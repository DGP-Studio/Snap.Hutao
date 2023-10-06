// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Auth;

/// <summary>
/// 授权客户端
/// </summary>
[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class AuthClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<BindingClient> logger;
    private readonly HttpClient httpClient;

    [ApiInformation(Cookie = CookieType.SToken, Salt = SaltType.K2)]
    public async ValueTask<Response<ActionTicketWrapper>> GetActionTicketBySTokenAsync(string action, User user, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);
        string stoken = user.SToken?.GetValueOrDefault(Cookie.STOKEN) ?? string.Empty;

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.AuthActionTicket(action, stoken, user.Aid))
            .SetUserCookie(user, CookieType.SToken)
            .Get();

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen1, SaltType.K2, true).ConfigureAwait(false);

        Response<ActionTicketWrapper>? resp = await builder
            .TryCatchSendAsync<Response<ActionTicketWrapper>>(httpClient, logger, token)
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
        Response<ListWrapper<NameToken>>? resp = null;
        if (cookie.TryGetLoginTicket(out Cookie? loginTicketCookie))
        {
            string loginTicket = loginTicketCookie[Cookie.LOGIN_TICKET];
            string loginUid = loginTicketCookie[Cookie.LOGIN_UID];

            string url = isOversea
                ? ApiOsEndpoints.AuthMultiToken(loginTicket, loginUid)
                : ApiEndpoints.AuthMultiToken(loginTicket, loginUid);

            HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
                .SetRequestUri(url)
                .Get();

            resp = await builder
                .TryCatchSendAsync<Response<ListWrapper<NameToken>>>(httpClient, logger, token)
                .ConfigureAwait(false);
        }

        return Response.Response.DefaultIfNull(resp);
    }
}