// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Service.Abstraction;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.User;

[Service(ServiceLifetime.Singleton, typeof(IUserRepository))]
internal sealed partial class UserRepository : IUserRepository
{
    [GeneratedConstructor]
    public partial UserRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public void DeleteUserById(Guid id)
    {
        this.DeleteByInnerId(id);
    }

    public ImmutableArray<Model.Entity.User> GetUserImmutableArray()
    {
        return this.ImmutableArray();
    }

    public void UpdateUser(Model.Entity.User user)
    {
        this.Update(user);
    }

    public void ClearUserSelection()
    {
        this.Execute(dbSet => dbSet.ExecuteUpdate(update => update.SetProperty(user => user.IsSelected, user => false)));
    }
}