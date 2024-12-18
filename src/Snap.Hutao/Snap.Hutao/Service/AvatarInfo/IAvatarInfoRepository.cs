// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Abstraction;
using System.Collections.Immutable;
using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;

namespace Snap.Hutao.Service.AvatarInfo;

internal interface IAvatarInfoRepository : IRepository<EntityAvatarInfo>
{
    void RemoveAvatarInfoRangeByUid(string uid);

    ImmutableArray<EntityAvatarInfo> GetAvatarInfoImmutableArrayByUid(string uid);
}