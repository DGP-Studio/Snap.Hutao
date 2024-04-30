// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Abstraction;
using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;

namespace Snap.Hutao.Service.AvatarInfo;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IAvatarInfoDbService))]
internal sealed partial class AvatarInfoDbService : IAvatarInfoDbService
{
    private readonly IServiceProvider serviceProvider;

    public IServiceProvider ServiceProvider { get => serviceProvider; }

    public List<EntityAvatarInfo> GetAvatarInfoListByUid(string uid)
    {
        return this.List(i => i.Uid == uid);
    }

    public ValueTask<List<EntityAvatarInfo>> GetAvatarInfoListByUidAsync(string uid, CancellationToken token = default)
    {
        return this.ListAsync(i => i.Uid == uid, token);
    }

    public void RemoveAvatarInfoRangeByUid(string uid)
    {
        this.Delete(i => i.Uid == uid);
    }

    public async ValueTask RemoveAvatarInfoRangeByUidAsync(string uid, CancellationToken token = default)
    {
        await this.DeleteAsync(i => i.Uid == uid, token).ConfigureAwait(false);
    }
}