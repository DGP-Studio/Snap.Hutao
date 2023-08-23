// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Geetest;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Verification;

/// <summary>
/// CardApi验证器
/// </summary>
[HighQuality]
[Injection(InjectAs.Transient)]
internal sealed class SlientGeetestCardVerifier : IGeetestCardVerifier
{
    private readonly CardClient cardClient;
    private readonly GeetestClient geetestClient;

    /// <summary>
    /// 构造一个新的CardApi验证器
    /// </summary>
    /// <param name="cardClient">card客户端</param>
    /// <param name="geetestClient">极验客户端</param>
    public SlientGeetestCardVerifier(CardClient cardClient, GeetestClient geetestClient)
    {
        this.cardClient = cardClient;
        this.geetestClient = geetestClient;
    }

    /// <summary>
    /// 尝试获取Xrpc使用的流水号
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>流水号</returns>
    public async ValueTask<string?> TryValidateXrpcChallengeAsync(User user, CancellationToken token)
    {
        Response.Response<VerificationRegistration> registrationResponse = await cardClient.CreateVerificationAsync(user, token).ConfigureAwait(false);
        if (registrationResponse.IsOk())
        {
            VerificationRegistration registration = registrationResponse.Data;

            await geetestClient.GetTypeAsync(registration.Gt).ConfigureAwait(false);
            GeetestResult<GeetestData>? ajax = await geetestClient.GetAjaxAsync(registration).ConfigureAwait(false);

            if (ajax?.Data.Validate is { } validate)
            {
                Response.Response<VerificationResult> verifyResponse = await cardClient.VerifyVerificationAsync(registration.Challenge, validate, token).ConfigureAwait(false);
                if (verifyResponse.IsOk())
                {
                    VerificationResult result = verifyResponse.Data;

                    if (result.Challenge is not null)
                    {
                        return result.Challenge;
                    }
                }
            }
        }

        return default;
    }
}