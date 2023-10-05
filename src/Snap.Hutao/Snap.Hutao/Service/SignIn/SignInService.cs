// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
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
        ISignInClient signInClient = serviceProvider
            .GetRequiredService<IOverseaSupportFactory<ISignInClient>>()
            .Create(userAndUid.User.IsOversea);

        Response<Reward> rewardResponse = await signInClient.GetRewardAsync(userAndUid.User, token).ConfigureAwait(false);

        if (rewardResponse.IsOk())
        {
            Response<SignInResult> resultResponse = await signInClient.SignAsync(userAndUid, token).ConfigureAwait(false);

            if (resultResponse.IsOk(showInfoBar: false))
            {
                Response<SignInRewardInfo> infoResponse = await signInClient.GetInfoAsync(userAndUid, token).ConfigureAwait(false);
                if (infoResponse.IsOk())
                {
                    int index = infoResponse.Data.TotalSignDay - 1;
                    Award award = rewardResponse.Data.Awards[index];
                    return new(true, SH.ServiceSignInSuccessRewardFormat.Format(award.Name, award.Count));
                }
                else
                {
                    return new(false, SH.ServiceSignInInfoRequestFailed);
                }
            }
            else
            {
                string message = resultResponse.Message;

                if (resultResponse.ReturnCode == (int)KnownReturnCode.AlreadySignedIn)
                {
                    return new(true, message);
                }

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