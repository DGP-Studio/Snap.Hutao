// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hutao.Geetest;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

/// <summary>
/// Global签到客户端
/// </summary>
[ConstructorGenerated(ResolveHttpClient = true)]
[UseDynamicSecret]
[HttpClient(HttpClientConfiguration.Default)]
[PrimaryHttpMessageHandler(UseCookies = false)]
internal sealed partial class SignInClientOversea : ISignInClient
{
    private readonly HttpClient httpClient;
    private readonly HomaGeetestClient homaGeetestClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<SignInClient> logger;

    public async ValueTask<Response<Reward>> GetRewardAsync(Model.Entity.User user, CancellationToken token = default)
    {
        Response<Reward>? resp = await httpClient
            .SetUser(user, CookieType.CookieToken)
            .TryCatchGetFromJsonAsync<Response<Reward>>(ApiOsEndpoints.SignInRewardHome, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<SignInResult>> SignAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        Response<SignInResult>? resp = await httpClient
            .SetUser(userAndUid.User, CookieType.CookieToken)
            .TryCatchPostAsJsonAsync<SignInData, Response<SignInResult>>(ApiOsEndpoints.SignInRewardSign, new(userAndUid.Uid, true), options, logger, token)
            .ConfigureAwait(false);

        if (resp is { Data: { Success: 1, Gt: string gt, Challenge: string originChallenge } })
        {
            GeetestResponse verifyResponse = await homaGeetestClient.VerifyAsync(gt, originChallenge, token).ConfigureAwait(false);

            if (verifyResponse is { Code: 0, Data: { Validate: string validate, Challenge: string challenge } })
            {
                resp = await httpClient
                    .SetUser(userAndUid.User, CookieType.CookieToken)
                    .SetXrpcChallenge(challenge, validate)
                    .TryCatchPostAsJsonAsync<SignInData, Response<SignInResult>>(ApiEndpoints.SignInRewardSign, new(userAndUid.Uid, true), options, logger, token)
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
