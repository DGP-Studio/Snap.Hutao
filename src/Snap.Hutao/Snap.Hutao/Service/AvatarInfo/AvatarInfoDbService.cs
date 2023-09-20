// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Database;
using EntityAvatarInfo = Snap.Hutao.Model.Entity.AvatarInfo;

namespace Snap.Hutao.Service.AvatarInfo;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IAvatarInfoDbService))]
internal sealed partial class AvatarInfoDbService : IAvatarInfoDbService
{
    private readonly IServiceProvider serviceProvider;

    public List<EntityAvatarInfo> GetAvatarInfoListByUid(string uid)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return appDbContext.AvatarInfos.AsNoTracking().Where(i => i.Uid == uid).ToList();
        }
    }

    public async ValueTask<List<EntityAvatarInfo>> GetAvatarInfoListByUidAsync(string uid)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await appDbContext.AvatarInfos
                .AsNoTracking()
                .Where(i => i.Uid == uid)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }

    public void RemoveAvatarInfoRangeByUid(string uid)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.AvatarInfos.ExecuteDeleteWhere(i => i.Uid == uid);
        }
    }

    public async ValueTask RemoveAvatarInfoRangeByUidAsync(string uid)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.AvatarInfos.ExecuteDeleteWhereAsync(i => i.Uid == uid).ConfigureAwait(false);
        }
    }
}