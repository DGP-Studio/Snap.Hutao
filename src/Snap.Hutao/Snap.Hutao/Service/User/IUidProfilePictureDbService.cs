// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.User;

internal interface IUidProfilePictureDbService : IAppDbService<UidProfilePicture>
{
    ValueTask<UidProfilePicture?> SingleUidProfilePictureOrDefaultByUidAsync(string uid, CancellationToken token = default);

    ValueTask UpdateUidProfilePictureAsync(UidProfilePicture profilePicture, CancellationToken token = default);

    ValueTask DeleteUidProfilePictureByUidAsync(string uid, CancellationToken token = default);
}
