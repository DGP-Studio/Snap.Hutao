// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal interface ISignInClient
{
    ValueTask<Response<SignInRewardInfo>> GetInfoAsync(UserAndUid userAndUid, CancellationToken token = default);

    ValueTask<Response<Reward>> GetRewardAsync(Model.Entity.User user, CancellationToken token = default);

    ValueTask<Response<SignInResult>> SignAsync(UserAndUid userAndUid, CancellationToken token = default);

    ValueTask<Response<SignInRewardReSignInfo>> GetResignInfoAsync(UserAndUid userAndUid, CancellationToken token = default);

    ValueTask<Response<SignInResult>> ReSignAsync(UserAndUid userAndUid, CancellationToken token = default);
}