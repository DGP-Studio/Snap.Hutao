// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Passport;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc3)]
internal sealed partial class PassportClientOversea : IPassportClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<PassportClientOversea> logger;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<UidCookieToken>> GetCookieAccountInfoBySTokenAsync(User user, CancellationToken token = default)
    {
        string? stoken = user.SToken?.GetValueOrDefault(Cookie.STOKEN);
        ArgumentException.ThrowIfNullOrEmpty(stoken);
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);
        STokenWrapper data = new(stoken, user.Aid);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.AccountGetCookieTokenBySToken)
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .PostJson(data);

        Response<UidCookieToken>? resp = await builder
            .TryCatchSendAsync<Response<UidCookieToken>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<LTokenWrapper>> GetLTokenBySTokenAsync(User user, CancellationToken token = default)
    {
        string? stoken = user.SToken?.GetValueOrDefault(Cookie.STOKEN);
        ArgumentException.ThrowIfNullOrEmpty(stoken);
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);
        STokenWrapper data = new(stoken, user.Aid);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.AccountGetLTokenBySToken)
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .PostJson(data);

        Response<LTokenWrapper>? resp = await builder
            .TryCatchSendAsync<Response<LTokenWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}