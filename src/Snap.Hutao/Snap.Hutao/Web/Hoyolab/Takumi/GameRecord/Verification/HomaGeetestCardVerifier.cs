// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Hutao.Geetest;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Verification;

[ConstructorGenerated]
[Injection(InjectAs.Transient, typeof(IGeetestCardVerifier), Key = GeetestCardVerifierType.Custom)]
internal sealed partial class HomaGeetestCardVerifier : IGeetestCardVerifier
{
    private readonly HomaGeetestClient homaGeetestClient;
    private readonly IInfoBarService infoBarService;
    private readonly CardClient cardClient;

    public async ValueTask<string?> TryValidateXrpcChallengeAsync(User user, CardVerifiationHeaders headers, CancellationToken token)
    {
        Response<VerificationRegistration> registrationResponse = await cardClient.CreateVerificationAsync(user, headers, token).ConfigureAwait(false);
        if (!ResponseValidator.TryValidate(registrationResponse, infoBarService, out VerificationRegistration? registration))
        {
            return default;
        }

        GeetestResponse response = await homaGeetestClient.VerifyAsync(registration.Gt, registration.Challenge, token).ConfigureAwait(false);

        if (response is not { Code: 0, Data.Validate: { } validate })
        {
            return default;
        }

        Response<VerificationResult> verifyResponse = await cardClient.VerifyVerificationAsync(user, headers, registration.Challenge, validate, token).ConfigureAwait(false);
        if (!ResponseValidator.TryValidate(verifyResponse, infoBarService, out VerificationResult? result))
        {
            return default;
        }

        if (result.Challenge is not null)
        {
            return result.Challenge;
        }

        return default;
    }
}