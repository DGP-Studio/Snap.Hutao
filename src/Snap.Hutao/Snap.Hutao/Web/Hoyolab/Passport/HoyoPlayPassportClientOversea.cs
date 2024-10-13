// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Endpoint.Hoyolab;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Globalization;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Passport;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc6)]
internal sealed partial class HoyoPlayPassportClientOversea : IHoyoPlayPassportClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<HoyoPlayPassportClientOversea> logger;
    [FromKeyed(ApiEndpointsKind.Oversea)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<AuthTicketWrapper>> CreateAuthTicketAsync(User user, CancellationToken token = default)
    {
        string? stoken = user.SToken?.GetValueOrDefault(Cookie.STOKEN);
        ArgumentException.ThrowIfNullOrEmpty(stoken);
        ArgumentException.ThrowIfNullOrEmpty(user.Mid);

        AuthTicketRequestOversea data = new()
        {
            BizName = "hk4e_global",
            Mid = user.Mid,
            SToken = stoken,
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountCreateAuthTicketByGameBiz())
            .PostJson(data);

        Response<AuthTicketWrapper>? resp = await builder
            .SendAsync<Response<AuthTicketWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}