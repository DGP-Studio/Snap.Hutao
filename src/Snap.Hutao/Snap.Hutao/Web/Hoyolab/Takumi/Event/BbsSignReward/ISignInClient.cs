// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

/// <summary>
/// 每日签到客户端接口
/// </summary>
internal interface ISignInClient
{
    ValueTask<Response<SignInRewardInfo>> GetInfoAsync(UserAndUid userAndUid, CancellationToken token = default(CancellationToken));

    ValueTask<Response<Reward>> GetRewardAsync(Model.Entity.User user, CancellationToken token = default);

    ValueTask<Response<SignInResult>> SignAsync(UserAndUid userAndUid, CancellationToken token = default);
}