// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hutao.Geetest;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Verification;

[ConstructorGenerated]
[Injection(InjectAs.Transient)]
internal sealed partial class HomaGeetestCardVerifier : IGeetestCardVerifier
{
    private readonly CardClient cardClient;
    private readonly HomaGeetestClient homaGeetestClient;

    public async ValueTask<string?> TryValidateXrpcChallengeAsync(User user, CancellationToken token)
    {
        Response.Response<VerificationRegistration> registrationResponse = await cardClient.CreateVerificationAsync(user, token).ConfigureAwait(false);
        if (registrationResponse.IsOk())
        {
            VerificationRegistration registration = registrationResponse.Data;

            GeetestResponse response = await homaGeetestClient.VerifyAsync(registration.Gt, registration.Challenge, token).ConfigureAwait(false);

            if (response is { Code: 0, Data.Validate: string validate })
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