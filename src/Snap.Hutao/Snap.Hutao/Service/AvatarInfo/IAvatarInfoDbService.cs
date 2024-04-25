// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Abstraction;
using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;

namespace Snap.Hutao.Service.AvatarInfo;

internal interface IAvatarInfoDbService : IAppDbService<EntityAvatarInfo>
{
    void RemoveAvatarInfoRangeByUid(string uid);

    List<EntityAvatarInfo> GetAvatarInfoListByUid(string uid);

    ValueTask<List<EntityAvatarInfo>> GetAvatarInfoListByUidAsync(string uid, CancellationToken token = default);

    ValueTask RemoveAvatarInfoRangeByUidAsync(string uid, CancellationToken token = default);
}