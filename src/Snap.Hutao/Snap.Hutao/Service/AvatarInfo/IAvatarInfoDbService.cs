// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;

namespace Snap.Hutao.Service.AvatarInfo;

internal interface IAvatarInfoDbService
{
    void DeleteAvatarInfoRangeByUid(string uid);

    List<EntityAvatarInfo> GetAvatarInfoListByUid(string uid);
}