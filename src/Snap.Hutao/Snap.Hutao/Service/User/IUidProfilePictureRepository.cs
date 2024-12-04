// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.User;

internal interface IUidProfilePictureRepository : IRepository<UidProfilePicture>
{
    UidProfilePicture? SingleUidProfilePictureOrDefaultByUid(string uid);

    void UpdateUidProfilePicture(UidProfilePicture profilePicture);

    void DeleteUidProfilePictureByUid(string uid);
}