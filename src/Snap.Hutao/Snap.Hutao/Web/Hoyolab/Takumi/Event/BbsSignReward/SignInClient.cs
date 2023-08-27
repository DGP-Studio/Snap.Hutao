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
/// 签到客户端
/// </summary>
[ConstructorGenerated(ResolveHttpClient = true)]
[UseDynamicSecret]
[HttpClient(HttpClientConfiguration.XRpc)]
[PrimaryHttpMessageHandler(UseCookies = false)]
internal sealed partial class SignInClient
{
    private readonly HttpClient httpClient;
    private readonly HomaGeetestClient homaGeetestClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<SignInClient> logger;

    public async ValueTask<Response<SignInRewardInfo>> GetInfoAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        Response<SignInRewardInfo>? resp = await httpClient
            .SetUser(userAndUid.User, CookieType.CookieToken)
            .UseDynamicSecret(DynamicSecretVersion.Gen2, SaltType.LK2, true)
            .TryCatchGetFromJsonAsync<Response<SignInRewardInfo>>(ApiEndpoints.SignInRewardInfo(userAndUid.Uid), options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<SignInRewardReSignInfo>> GetResignInfoAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        Response<SignInRewardReSignInfo>? resp = await httpClient
            .SetUser(userAndUid.User, CookieType.CookieToken)
            .UseDynamicSecret(DynamicSecretVersion.Gen2, SaltType.LK2, true)
            .TryCatchGetFromJsonAsync<Response<SignInRewardReSignInfo>>(ApiEndpoints.SignInRewardResignInfo(userAndUid.Uid), options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<Reward>> GetRewardAsync(Model.Entity.User user, CancellationToken token = default)
    {
        Response<Reward>? resp = await httpClient
            .SetUser(user, CookieType.CookieToken)
            .TryCatchGetFromJsonAsync<Response<Reward>>(ApiEndpoints.SignInRewardHome, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<SignInResult>> ReSignAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        SignInData data = new(userAndUid.Uid);

        Response<SignInResult>? resp = await httpClient
            .SetUser(userAndUid.User, CookieType.CookieToken)
            .UseDynamicSecret(DynamicSecretVersion.Gen2, SaltType.LK2, true)
            .TryCatchPostAsJsonAsync<SignInData, Response<SignInResult>>(ApiEndpoints.SignInRewardReSign, data, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<SignInResult>> SignAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        Response<SignInResult>? resp = await httpClient
            .SetUser(userAndUid.User, CookieType.CookieToken)
            .UseDynamicSecret(DynamicSecretVersion.Gen1, SaltType.LK2, false)
            .TryCatchPostAsJsonAsync<SignInData, Response<SignInResult>>(ApiEndpoints.SignInRewardSign, new SignInData(userAndUid.Uid), options, logger, token)
            .ConfigureAwait(false);

        if (resp is { Data: { Success: 1, Gt: string gt, Challenge: string challenge } })
        {
            GeetestResponse verifyResponse = await homaGeetestClient.VerifyAsync(gt, challenge, token).ConfigureAwait(false);

            if (verifyResponse is { Code: 0, Data.Validate: string validate })
            {
                resp = await httpClient
                    .SetUser(userAndUid.User, CookieType.CookieToken)
                    .SetXrpcChallenge(challenge, validate)
                    .UseDynamicSecret(DynamicSecretVersion.Gen1, SaltType.LK2, false)
                    .TryCatchPostAsJsonAsync<SignInData, Response<SignInResult>>(ApiEndpoints.SignInRewardSign, new SignInData(userAndUid.Uid), options, logger, token)
                    .ConfigureAwait(false);
            }
        }

        return Response.Response.DefaultIfNull(resp);
    }
}