// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.User;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUserRepository))]
internal sealed partial class UserRepository : IUserRepository
{
    private readonly IServiceProvider serviceProvider;

    public IServiceProvider ServiceProvider { get => serviceProvider; }

    public void DeleteUserById(Guid id)
    {
        this.DeleteByInnerId(id);
    }

    public List<Model.Entity.User> GetUserList()
    {
        return this.List();
    }

    public void UpdateUser(Model.Entity.User user)
    {
        this.Update(user);
    }

    public void RemoveAllUsers()
    {
        this.Delete();
    }

    public void ClearUserSelection()
    {
        this.Execute(dbset => dbset.ExecuteUpdate(update => update.SetProperty(user => user.IsSelected, user => false)));
    }
}