// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.DataSigning;
using Snap.Hutao.Web.Hutao.Geetest;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

/// <summary>
/// 签到客户端
/// </summary>
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc)]
[PrimaryHttpMessageHandler(UseCookies = false)]
internal sealed partial class SignInClient : ISignInClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly HomaGeetestClient homaGeetestClient;
    private readonly CultureOptions cultureOptions;
    private readonly ILogger<SignInClient> logger;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<ExtraAwardInfo>> GetExtraAwardInfoAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.LunaExtraAward(userAndUid.Uid, cultureOptions.LanguageCode))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
            .SetHeader("x-rpc-signgame", "hk4e")
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen1, SaltType.LK2, true).ConfigureAwait(false);

        Response<ExtraAwardInfo>? resp = await builder
            .TryCatchSendAsync<Response<ExtraAwardInfo>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<SignInRewardInfo>> GetInfoAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.LunaInfo(userAndUid.Uid, cultureOptions.LanguageCode))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
            .SetHeader("x-rpc-signgame", "hk4e")
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen1, SaltType.LK2, true).ConfigureAwait(false);

        Response<SignInRewardInfo>? resp = await builder
            .TryCatchSendAsync<Response<SignInRewardInfo>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<SignInRewardReSignInfo>> GetResignInfoAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.LunaResignInfo(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
            .SetHeader("x-rpc-signgame", "hk4e")
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen1, SaltType.LK2, true).ConfigureAwait(false);

        Response<SignInRewardReSignInfo>? resp = await builder
            .TryCatchSendAsync<Response<SignInRewardReSignInfo>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<Reward>> GetRewardAsync(Model.Entity.User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.LunaHome(cultureOptions.LanguageCode))
            .SetUserCookieAndFpHeader(user, CookieType.CookieToken)
            .SetHeader("x-rpc-signgame", "hk4e")
            .Get();

        Response<Reward>? resp = await builder
            .TryCatchSendAsync<Response<Reward>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<SignInResult>> ReSignAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.LunaReSign)
            .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
            .SetHeader("x-rpc-signgame", "hk4e")
            .PostJson(new SignInData(userAndUid.Uid, false));

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen1, SaltType.LK2, true).ConfigureAwait(false);

        Response<SignInResult>? resp = await builder
            .TryCatchSendAsync<Response<SignInResult>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<SignInResult>> SignAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.LunaSign)
            .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
            .SetHeader("x-rpc-signgame", "hk4e")
            .PostJson(new SignInData(userAndUid.Uid, false));

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen1, SaltType.LK2, true).ConfigureAwait(false);

        Response<SignInResult>? resp = await builder
            .TryCatchSendAsync<Response<SignInResult>>(httpClient, logger, token)
            .ConfigureAwait(false);

        if (resp is { Data: { Success: 1, Gt: string gt, Challenge: string originChallenge } })
        {
            GeetestResponse verifyResponse = await homaGeetestClient.VerifyAsync(gt, originChallenge, token).ConfigureAwait(false);

            if (verifyResponse is { Code: 0, Data: { Validate: string validate, Challenge: string challenge } })
            {
                HttpRequestMessageBuilder verifiedBuilder = httpRequestMessageBuilderFactory.Create()
                    .SetRequestUri(ApiEndpoints.LunaSign)
                    .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
                    .SetHeader("x-rpc-signgame", "hk4e")
                    .SetXrpcChallenge(challenge, validate)
                    .PostJson(new SignInData(userAndUid.Uid, false));

                await verifiedBuilder.SignDataAsync(DataSignAlgorithmVersion.Gen1, SaltType.LK2, true).ConfigureAwait(false);

                resp = await verifiedBuilder
                    .TryCatchSendAsync<Response<SignInResult>>(httpClient, logger, token)
                    .ConfigureAwait(false);
            }
            else
            {
                resp.ReturnCode = resp.Data.RiskCode;
                resp.Message = SH.ServiceSignInRiskVerificationFailed;
            }
        }

        return Response.Response.DefaultIfNull(resp);
    }
}