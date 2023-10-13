﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hutao.Geetest;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

/// <summary>
/// Global签到客户端
/// </summary>
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
[PrimaryHttpMessageHandler(UseCookies = false)]
internal sealed partial class SignInClientOversea : ISignInClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly HomaGeetestClient homaGeetestClient;
    private readonly ILogger<SignInClient> logger;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<SignInRewardInfo>> GetInfoAsync(UserAndUid userAndUid, CancellationToken token = default(CancellationToken))
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.SignInRewardInfo(userAndUid.Uid))
            .SetUserCookie(userAndUid, CookieType.CookieToken)
            .Get();

        Response<SignInRewardInfo>? resp = await builder
            .TryCatchSendAsync<Response<SignInRewardInfo>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<Reward>> GetRewardAsync(Model.Entity.User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.SignInRewardHome)
            .SetUserCookie(user, CookieType.CookieToken)
            .Get();

        Response<Reward>? resp = await builder
            .TryCatchSendAsync<Response<Reward>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<SignInResult>> SignAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.SignInRewardSign)
            .SetUserCookie(userAndUid, CookieType.CookieToken)
            .PostJson(new SignInData(userAndUid.Uid, true));

        Response<SignInResult>? resp = await builder
            .TryCatchSendAsync<Response<SignInResult>>(httpClient, logger, token)
            .ConfigureAwait(false);

        if (resp is { Data: { Success: 1, Gt: string gt, Challenge: string originChallenge } })
        {
            GeetestResponse verifyResponse = await homaGeetestClient.VerifyAsync(gt, originChallenge, token).ConfigureAwait(false);

            if (verifyResponse is { Code: 0, Data: { Validate: string validate, Challenge: string challenge } })
            {
                HttpRequestMessageBuilder verifiedBuilder = httpRequestMessageBuilderFactory.Create()
                    .SetRequestUri(ApiOsEndpoints.SignInRewardSign)
                    .SetUserCookie(userAndUid, CookieType.CookieToken)
                    .SetXrpcChallenge(challenge, validate)
                    .PostJson(new SignInData(userAndUid.Uid, true));

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