// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.User;

[Service(ServiceLifetime.Singleton, typeof(IUidProfilePictureRepository))]
internal sealed partial class UidProfilePictureRepository : IUidProfilePictureRepository
{
    [GeneratedConstructor]
    public partial UidProfilePictureRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public UidProfilePicture? SingleUidProfilePictureOrDefaultByUid(string uid)
    {
        try
        {
            return this.Query(query => query.SingleOrDefault(n => n.Uid == uid));
        }
        catch
        {
            this.Delete(n => n.Uid == uid);
            return default;
        }
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