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
    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="user">用户信息</param>
    /// <param name="token">token</param>
    /// <returns>结果</returns>
    ValueTask<Response<Reward>> GetRewardAsync(Model.Entity.User user, CancellationToken token = default);

    /// <summary>
    /// 签到
    /// </summary>
    /// <param name="userAndUid">用户信息</param>
    /// <param name="token">token</param>
    /// <returns>签到结果</returns>
    ValueTask<Response<SignInResult>> SignAsync(UserAndUid userAndUid, CancellationToken token = default);
}
