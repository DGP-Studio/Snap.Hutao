// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.DataSigning;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc)]
[PrimaryHttpMessageHandler(UseCookies = false)]
internal sealed partial class BindingClient2
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<BindingClient2> logger;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<ListWrapper<UserGameRole>>> GetUserGameRolesBySTokenAsync(User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.AccountGetCookieTokenBySToken)
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .SetReferer(ApiEndpoints.AppMihoyoReferer)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen1, SaltType.LK2, true).ConfigureAwait(false);

        Response<ListWrapper<UserGameRole>>? resp = await builder
            .TryCatchSendAsync<Response<ListWrapper<UserGameRole>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<GameAuthKey>> GenerateAuthenticationKeyAsync(User user, GenAuthKeyData data, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.BindingGenAuthKey)
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .SetReferer(ApiEndpoints.AppMihoyoReferer)
            .PostJson(data);

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen1, SaltType.LK2, true).ConfigureAwait(false);

        Response<GameAuthKey>? resp = await builder
            .TryCatchSendAsync<Response<GameAuthKey>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}