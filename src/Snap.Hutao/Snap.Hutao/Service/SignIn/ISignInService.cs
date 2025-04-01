// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.Service.SignIn;

internal interface ISignInService
{
    ValueTask<bool> ClaimSignInRewardAsync(UserAndUid userAndUid, CancellationToken token = default);

    ValueTask<bool> ClaimResignInRewardAsync(UserAndUid userAndUid, CancellationToken token = default);
}