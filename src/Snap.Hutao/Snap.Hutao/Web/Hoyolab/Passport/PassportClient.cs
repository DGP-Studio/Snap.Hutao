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

namespace Snap.Hutao.Web.Hoyolab.Passport;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc2)]
internal sealed partial class PassportClient : IPassportClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<PassportClient2> logger;
    [FromKeyed(ApiEndpointsKind.Chinese)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<UidCookieToken>> GetCookieAccountInfoBySTokenAsync(User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountGetCookieTokenBySToken())
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.PROD, true).ConfigureAwait(false);

        Response<UidCookieToken>? resp = await builder
            .SendAsync<Response<UidCookieToken>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<LTokenWrapper>> GetLTokenBySTokenAsync(User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountGetLTokenBySToken())
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.PROD, true).ConfigureAwait(false);

        Response<LTokenWrapper>? resp = await builder
            .SendAsync<Response<LTokenWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}