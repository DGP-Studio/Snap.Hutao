// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.User;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUidProfilePictureRepository))]
internal sealed partial class UidProfilePictureRepository : IUidProfilePictureRepository
{
    private readonly IServiceProvider serviceProvider;

    public IServiceProvider ServiceProvider { get => serviceProvider; }

    public UidProfilePicture? SingleUidProfilePictureOrDefaultByUid(string uid)
    {
        return this.Query(query => query.SingleOrDefault(n => n.Uid == uid));
    }

    public void UpdateUidProfilePicture(UidProfilePicture profilePicture)
    {
        this.Update(profilePicture);
    }

    public void DeleteUidProfilePictureByUid(string uid)
    {
        this.Delete(profilePicture => profilePicture.Uid == uid);
    }
}