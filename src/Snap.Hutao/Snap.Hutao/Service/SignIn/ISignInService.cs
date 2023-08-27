// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.Service.SignIn;

internal interface ISignInService
{
    ValueTask<ValueResult<bool, string>> ClaimRewardAsync(UserAndUid userAndUid, CancellationToken token = default);
}