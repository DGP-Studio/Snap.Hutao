// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Endpoint.Hoyolab;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Passport;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc5)]
internal sealed partial class HoyoPlayPassportClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<HoyoPlayPassportClient> logger;
    [FromKeyed(ApiEndpointsKind.Chinese)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<AuthTicketWrapper>> CreateAuthTicketAsync(User user, CancellationToken token = default)
    {
        string? stoken = user.SToken?.GetValueOrDefault(Cookie.STOKEN);
        ArgumentException.ThrowIfNullOrEmpty(stoken);
        ArgumentException.ThrowIfNullOrEmpty(user.Mid);
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);

        AuthTicketRequest data = new()
        {
            GameBiz = "hk4e_cn",
            Mid = user.Mid,
            SToken = stoken,
            Uid = int.Parse(user.Aid),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountCreateAuthTicketByGameBiz())
            .SetUserCookieAndFpHeader(user, CookieType.LToken)
            .PostJson(data);

        Response<AuthTicketWrapper>? resp = await builder
            .SendAsync<Response<AuthTicketWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}