// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.User;

internal interface IUserRepository : IRepository<Model.Entity.User>
{
    void DeleteUserById(Guid id);

    void RemoveAllUsers();

    List<Model.Entity.User> GetUserList();

    void UpdateUser(Model.Entity.User user);

    void ClearUserSelection();
}