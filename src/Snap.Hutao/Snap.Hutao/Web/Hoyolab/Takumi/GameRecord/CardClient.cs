// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
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

    /// <summary>
    /// 注册验证码
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>注册结果</returns>
    public async ValueTask<Response<VerificationRegistration>> CreateVerificationAsync(User user, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.CardCreateVerification(false))
            .SetUserCookieAndFpHeader(user, CookieType.LToken)
            .Get();

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<VerificationRegistration>? resp = await builder
            .TryCatchSendAsync<Response<VerificationRegistration>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 二次验证
    /// </summary>
    /// <param name="challenge">流水号</param>
    /// <param name="validate">验证</param>
    /// <param name="token">取消令牌</param>
    /// <returns>验证结果</returns>
    public async ValueTask<Response<VerificationResult>> VerifyVerificationAsync(string challenge, string validate, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.CardVerifyVerification)
            .PostJson(new VerificationData(challenge, validate));

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

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

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.X6, false).ConfigureAwait(false);

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