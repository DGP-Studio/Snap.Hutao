// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.User;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUserGameRoleDbService))]
internal sealed partial class UserGameRoleDbService : IUserGameRoleDbService
{
    private readonly IServiceProvider serviceProvider;

    public IServiceProvider ServiceProvider { get => serviceProvider; }

    public ValueTask<bool> ContainsUidAsync(string uid, CancellationToken token = default)
    {
        return this.QueryAsync(query => query.AnyAsync(n => n.Uid == uid));
    }

    public ValueTask<UserGameRoleProfilePicture> GetUserGameRoleProfilePictureByUidAsync(string uid, CancellationToken token = default)
    {
        return this.QueryAsync(query => query.FirstAsync(n => n.Uid == uid));
    }

    public async ValueTask UpdateUserGameRoleProfilePictureAsync(UserGameRoleProfilePicture profilePicture, CancellationToken token = default)
    {
        await this.UpdateAsync(profilePicture, token).ConfigureAwait(false);
    }

    public async ValueTask DeleteUserGameRoleProfilePictureByUidAsync(string uid, CancellationToken token = default)
    {
        await this.DeleteAsync(profilePicture => profilePicture.Uid == uid, token).ConfigureAwait(false);
    }
}
