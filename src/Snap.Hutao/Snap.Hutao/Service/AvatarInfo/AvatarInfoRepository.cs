// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Abstraction;
using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;

namespace Snap.Hutao.Service.AvatarInfo;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IAvatarInfoRepository))]
internal sealed partial class AvatarInfoRepository : IAvatarInfoRepository
{
    public partial IServiceProvider ServiceProvider { get; }

    public List<EntityAvatarInfo> GetAvatarInfoListByUid(string uid)
    {
        return this.List(i => i.Uid == uid);
    }

    public void RemoveAvatarInfoRangeByUid(string uid)
    {
        this.Delete(i => i.Uid == uid);
    }
}