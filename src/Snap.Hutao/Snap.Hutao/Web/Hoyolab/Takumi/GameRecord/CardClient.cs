// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Widget;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// 卡片客户端
/// </summary>
[UseDynamicSecret]
[HttpClient(HttpClientConfigration.XRpc)]
public class CardClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<CardClient> logger;

    /// <summary>
    /// 构造一个新的卡片客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">选项</param>
    /// <param name="logger">日志器</param>
    public CardClient(HttpClient httpClient, JsonSerializerOptions options, ILogger<CardClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 注册验证码
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>注册结果</returns>
    public async Task<VerificationRegistration?> CreateVerificationAsync(User user, CancellationToken token)
    {
        Response<VerificationRegistration>? resp = await httpClient
            .SetUser(user, CookieType.Ltoken)
            .UseDynamicSecret(DynamicSecretVersion.Gen2, SaltType.X4, false)
            .TryCatchGetFromJsonAsync<Response<VerificationRegistration>>(ApiEndpoints.CardCreateVerification, options, logger, token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 二次验证
    /// </summary>
    /// <param name="challenge">流水号</param>
    /// <param name="validate">验证</param>
    /// <param name="token">取消令牌</param>
    /// <returns>验证结果</returns>
    public async Task<VerificationResult?> VerifyVerificationAsync(string challenge, string validate, CancellationToken token)
    {
        VerificationData data = new(challenge, validate);

        Response<VerificationResult>? resp = await httpClient
            .TryCatchPostAsJsonAsync<VerificationData, Response<VerificationResult>>(ApiEndpoints.CardVerifyVerification, data, options, logger, token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 异步获取桌面小组件数据
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>桌面小组件数据</returns>
    [ApiInformation(Cookie = CookieType.Stoken, Salt = SaltType.X6)]
    public async Task<WidgetData?> GetWidgetDataAsync(User user, CancellationToken token)
    {
        Response<DataWrapper<WidgetData>>? resp = await httpClient
            .SetUser(user, CookieType.Stoken)
            .UseDynamicSecret(DynamicSecretVersion.Gen2, SaltType.X6, false)
            .TryCatchGetFromJsonAsync<Response<DataWrapper<WidgetData>>>(ApiEndpoints.CardWidgetData, options, logger, token)
            .ConfigureAwait(false);

        return resp?.Data?.Data;
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