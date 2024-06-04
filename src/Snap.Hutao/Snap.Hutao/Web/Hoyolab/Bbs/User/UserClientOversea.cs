// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class UserClientOversea : IUserClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<UserClientOversea> logger;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<UserFullInfoWrapper>> GetUserFullInfoAsync(Model.Entity.User user, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.UserFullInfoQuery(user.Aid))
            .SetUserCookieAndFpHeader(user, CookieType.LToken)
            .Get();

        Response<UserFullInfoWrapper>? resp = await builder
            .SendAsync<Response<UserFullInfoWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}