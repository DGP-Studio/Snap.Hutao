// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Endpoint.Hoyolab;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc)]
internal sealed partial class UserClient : IUserClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<UserClient> logger;
    [FromKeyed(ApiEndpointsKind.Chinese)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<UserFullInfoWrapper>> GetUserFullInfoAsync(Model.Entity.User user, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.UserFullInfoQuery(user.Aid))
            .SetReferer(apiEndpoints.BbsReferer())
            .Get();

        Response<UserFullInfoWrapper>? resp = await builder
            .SendAsync<Response<UserFullInfoWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}