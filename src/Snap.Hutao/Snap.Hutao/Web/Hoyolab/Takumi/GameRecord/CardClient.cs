// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DataSigning;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Verification;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// 卡片客户端
/// </summary>
[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc)]
internal sealed partial class CardClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<CardClient> logger;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<VerificationRegistration>> CreateVerificationAsync(User user, CardVerifiationHeaders headers, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.CardCreateVerification(true))
            .SetUserCookieAndFpHeader(user, CookieType.LToken)
            .SetHeader("x-rpc-challenge_game", $"{headers.ChallengeGame}")
            .SetHeader("x-rpc-challenge_path", headers.ChallengePath)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<VerificationRegistration>? resp = await builder
            .TryCatchSendAsync<Response<VerificationRegistration>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<VerificationResult>> VerifyVerificationAsync(CardVerifiationHeaders headers, string challenge, string validate, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.CardVerifyVerification)
            .SetHeader("x-rpc-challenge_game", $"{headers.ChallengeGame}")
            .SetHeader("x-rpc-challenge_path", headers.ChallengePath)
            .PostJson(new VerificationData(challenge, validate));

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<VerificationResult>? resp = await builder
            .TryCatchSendAsync<Response<VerificationResult>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取桌面小组件数据
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>桌面小组件数据</returns>
    [ApiInformation(Cookie = CookieType.SToken, Salt = SaltType.X6)]
    public async ValueTask<Response<DailyNote.WidgetDailyNote>> GetWidgetDataAsync(User user, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.CardWidgetData2)
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X6, false).ConfigureAwait(false);

        Response<DailyNote.WidgetDailyNote>? resp = await builder
            .TryCatchSendAsync<Response<DailyNote.WidgetDailyNote>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    private class VerificationData
    {
        public VerificationData(string challenge, string validate)
        {
            GeetestChallenge = challenge;
            GeetestValidate = validate;
            GeetestSeccode = $"{validate}|jordan";
        }

        [JsonPropertyName("geetest_challenge")]
        public string GeetestChallenge { get; set; }

        [JsonPropertyName("geetest_validate")]
        public string GeetestValidate { get; set; }

        [JsonPropertyName("geetest_seccode")]
        public string GeetestSeccode { get; set; }
    }
}

internal sealed class CardVerifiationHeaders
{
    public int ChallengeGame { get; private set; }

    public string ChallengePath { get; private set; }

    public static CardVerifiationHeaders Create(string path)
    {
        return new()
        {
            ChallengeGame = 2,
            ChallengePath = path,
        };
    }
}