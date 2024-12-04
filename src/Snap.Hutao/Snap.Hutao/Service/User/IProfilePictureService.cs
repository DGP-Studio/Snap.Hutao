// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.Binding;

namespace Snap.Hutao.Service.User;

internal interface IProfilePictureService
{
    ValueTask TryInitializeAsync(ViewModel.User.User user, CancellationToken token = default);

    ValueTask RefreshUserGameRoleAsync(UserGameRole userGameRole, CancellationToken token = default);
}