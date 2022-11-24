// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Geetest;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// CardApi验证器
/// </summary>
[Injection(InjectAs.Transient)]
internal class CardVerifier
{
    private readonly CardClient cardClient;
    private readonly GeetestClient geetestClient;

    /// <summary>
    /// 构造一个新的CardApi验证器
    /// </summary>
    /// <param name="cardClient">card客户端</param>
    /// <param name="geetestClient">极验客户端</param>
    public CardVerifier(CardClient cardClient, GeetestClient geetestClient)
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
    public async Task<string?> TryGetXrpcChallengeAsync(User user, CancellationToken token)
    {
        if (await cardClient.CreateVerificationAsync(user, token).ConfigureAwait(false) is VerificationRegistration registration)
        {
            _ = await geetestClient.GetTypeAsync(registration.Gt).ConfigureAwait(false);
            GeetestResult<GeetestData>? ajax = await geetestClient.GetAjaxAsync(registration).ConfigureAwait(false);

            if (ajax?.Data.Validate is string validate)
            {
                VerificationResult? result = await cardClient.VerifyVerificationAsync(registration.Challenge, validate, token).ConfigureAwait(false);

                if (result?.Challenge != null)
                {
                    return result.Challenge;
                }
            }
        }

        return null;
    }
}