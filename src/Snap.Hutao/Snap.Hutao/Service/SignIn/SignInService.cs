// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.SignIn;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(ISignInService))]
internal sealed partial class SignInService : ISignInService
{
    private readonly IServiceProvider serviceProvider;

    public async ValueTask<ValueResult<bool, string>> ClaimRewardAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        SignInClient signInClient = serviceProvider.GetRequiredService<SignInClient>();

        Response<Reward> rewardResponse = await signInClient.GetRewardAsync(userAndUid.User, token).ConfigureAwait(false);

        if (rewardResponse.IsOk())
        {
            Response<SignInResult> resultResponse = await signInClient.SignAsync(userAndUid, token).ConfigureAwait(false);

            if (resultResponse.IsOk())
            {
                int index = DateTimeOffset.Now.Day - 1;
                Award award = rewardResponse.Data.Awards[index];
                return new(true, SH.ServiceSignInSuccessRewardFormat.Format(award.Name, award.Count));
            }
            else
            {
                string message = resultResponse.Message;

                if (string.IsNullOrEmpty(message))
                {
                    message = $"RiskCode: {resultResponse.Data?.RiskCode}";
                }

                return new(false, SH.ServiceSignInClaimRewardFailedFormat.Format(message));
            }
        }
        else
        {
            return new(false, SH.ServiceSignInRewardListRequestFailed);
        }
    }
}