// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Endpoint.Hoyolab;
using Snap.Hutao.Web.Hoyolab.DataSigning;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Auth;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class AuthClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IApiEndpointsFactory apiEndpointsFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial AuthClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<ActionTicketWrapper>> GetActionTicketBySTokenAsync(string action, User user, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);
        string stoken = user.SToken?.GetValueOrDefault(Cookie.STOKEN) ?? string.Empty;

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(user.IsOversea).AuthActionTicket(action, stoken, user.Aid))
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen1, SaltType.K2, true).ConfigureAwait(false);

        Response<ActionTicketWrapper>? resp = await builder
            .SendAsync<Response<ActionTicketWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<ListWrapper<NameToken>>> GetMultiTokenByLoginTicketAsync(Cookie cookie, bool isOversea, CancellationToken token = default)
    {
        Response<ListWrapper<NameToken>>? resp = null;
        if (cookie.TryGetLoginTicket(out Cookie? loginTicketCookie))
        {
            string loginTicket = loginTicketCookie[Cookie.LOGIN_TICKET];
            string loginUid = loginTicketCookie[Cookie.LOGIN_UID];

            HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
                .SetRequestUri(apiEndpointsFactory.Create(isOversea).AuthMultiToken(loginTicket, loginUid))
                .Get();

            resp = await builder
                .SendAsync<Response<ListWrapper<NameToken>>>(httpClient, token)
                .ConfigureAwait(false);
        }

        return Response.Response.DefaultIfNull(resp);
    }
}