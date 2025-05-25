// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Win32;
using System.Data.Common;
using System.Diagnostics;

namespace Snap.Hutao.Service.User;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IUidProfilePictureRepository))]
internal sealed partial class UidProfilePictureRepository : IUidProfilePictureRepository
{
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