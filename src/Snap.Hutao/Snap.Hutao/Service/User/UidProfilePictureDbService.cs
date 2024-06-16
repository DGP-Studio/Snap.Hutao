// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.User;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUidProfilePictureDbService))]
internal sealed partial class UidProfilePictureDbService : IUidProfilePictureDbService
{
    private readonly IServiceProvider serviceProvider;

    public IServiceProvider ServiceProvider { get => serviceProvider; }

    public ValueTask<UidProfilePicture?> SingleUidProfilePictureOrDefaultByUidAsync(string uid, CancellationToken token = default)
    {
        return this.QueryAsync(query => query.SingleOrDefaultAsync(n => n.Uid == uid));
    }

    public async ValueTask UpdateUidProfilePictureAsync(UidProfilePicture profilePicture, CancellationToken token = default)
    {
        await this.UpdateAsync(profilePicture, token).ConfigureAwait(false);
    }

    public async ValueTask DeleteUidProfilePictureByUidAsync(string uid, CancellationToken token = default)
    {
        await this.DeleteAsync(profilePicture => profilePicture.Uid == uid, token).ConfigureAwait(false);
    }
}