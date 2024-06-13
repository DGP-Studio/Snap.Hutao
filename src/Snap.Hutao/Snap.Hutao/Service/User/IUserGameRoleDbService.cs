// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.User;

internal interface IUserGameRoleDbService : IAppDbService<UserGameRoleProfilePicture>
{
    ValueTask<bool> ContainsUidAsync(string uid, CancellationToken token = default);

    ValueTask<UserGameRoleProfilePicture> GetUserGameRoleProfilePictureByUidAsync(string uid, CancellationToken token = default);

    ValueTask UpdateUserGameRoleProfilePictureAsync(UserGameRoleProfilePicture profilePicture, CancellationToken token = default);

    ValueTask DeleteUserGameRoleProfilePictureByUidAsync(string uid, CancellationToken token = default);
}
