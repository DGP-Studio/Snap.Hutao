// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;

namespace Snap.Hutao.Service.AvatarInfo;

internal interface IAvatarInfoDbService
{
    void RemoveAvatarInfoRangeByUid(string uid);

    List<EntityAvatarInfo> GetAvatarInfoListByUid(string uid);

    ValueTask<List<EntityAvatarInfo>> GetAvatarInfoListByUidAsync(string uid);

    ValueTask RemoveAvatarInfoRangeByUidAsync(string uid);
}