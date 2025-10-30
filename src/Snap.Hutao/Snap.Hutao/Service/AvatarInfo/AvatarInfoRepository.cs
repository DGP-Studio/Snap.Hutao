// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Abstraction;
using System.Collections.Immutable;
using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;

namespace Snap.Hutao.Service.AvatarInfo;

[Service(ServiceLifetime.Singleton, typeof(IAvatarInfoRepository))]
internal sealed partial class AvatarInfoRepository : IAvatarInfoRepository
{
    [GeneratedConstructor]
    public partial AvatarInfoRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public ImmutableArray<EntityAvatarInfo> GetAvatarInfoImmutableArrayByUid(string uid)
    {
        return this.ImmutableArray(i => i.Uid == uid);
    }

    public void RemoveAvatarInfoRangeByUid(string uid)
    {
        this.Delete(i => i.Uid == uid);
    }
}