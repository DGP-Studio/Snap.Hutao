// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;

namespace Snap.Hutao.Service.User;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUserDbService))]
internal sealed partial class UserDbService : IUserDbService
{
    private readonly IServiceProvider serviceProvider;

    public async ValueTask DeleteUserByIdAsync(Guid id)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.Users.ExecuteDeleteWhereAsync(u => u.InnerId == id).ConfigureAwait(false);
        }
    }

    public async ValueTask<List<Model.Entity.User>> GetUserListAsync()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await appDbContext.Users.AsNoTracking().ToListAsync().ConfigureAwait(false);
        }
    }

    public async ValueTask UpdateUserAsync(Model.Entity.User user)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.Users.UpdateAndSaveAsync(user).ConfigureAwait(false);
        }
    }

    public async ValueTask AddUserAsync(Model.Entity.User user)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.Users.AddAndSaveAsync(user).ConfigureAwait(false);
        }
    }

    public async ValueTask DeleteUsersAsync()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.Users.ExecuteDeleteAsync().ConfigureAwait(false);
        }
    }
}