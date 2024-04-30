// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.User;

internal interface IUserDbService
{
    ValueTask DeleteUserByIdAsync(Guid id);

    ValueTask RemoveUsersAsync();

    ValueTask<List<Model.Entity.User>> GetUserListAsync();

    ValueTask UpdateUserAsync(Model.Entity.User user);

    ValueTask ClearUserSelectionAsync();
}