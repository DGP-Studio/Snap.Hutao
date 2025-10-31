// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Geetest;
using Snap.Hutao.Web.Endpoint.Hoyolab;
using Snap.Hutao.Web.Hoyolab.DataSigning;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Verification;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

[HttpClient(HttpClientConfiguration.XRpc)]
internal sealed partial class CardClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    [FromKeyed(ApiEndpointsKind.Chinese)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial CardClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<GeetestVerification>> CreateVerificationAsync(User user, CardVerifiationHeaders headers, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.CardCreateVerification(true))
            .SetUserCookieAndFpHeader(user, CookieType.Cookie)
            .SetHeader("x-rpc-challenge_game", $"{headers.ChallengeGame}")
            .SetHeader("x-rpc-challenge_path", headers.ChallengePath)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<GeetestVerification>? resp = await builder
            .SendAsync<Response<GeetestVerification>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<VerificationResult>> VerifyVerificationAsync(User user, CardVerifiationHeaders headers, string challenge, string validate, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.CardVerifyVerification())
            .SetUserCookieAndFpHeader(user, CookieType.Cookie)
            .SetHeader("x-rpc-challenge_game", $"{headers.ChallengeGame}")
            .SetHeader("x-rpc-challenge_path", headers.ChallengePath)
            .PostJson(new GeetestWebResponse(challenge, validate));

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<VerificationResult>? resp = await builder
            .SendAsync<Response<VerificationResult>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<DailyNote.WidgetDailyNote>> GetWidgetDataAsync(User user, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.CardWidgetData2())
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X6, false).ConfigureAwait(false);

        Response<DailyNote.WidgetDailyNote>? resp = await builder
            .SendAsync<Response<DailyNote.WidgetDailyNote>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}