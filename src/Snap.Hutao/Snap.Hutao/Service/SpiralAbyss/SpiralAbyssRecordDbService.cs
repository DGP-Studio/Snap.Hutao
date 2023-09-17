// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;

namespace Snap.Hutao.Service.SpiralAbyss;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(ISpiralAbyssRecordDbService))]
internal sealed partial class SpiralAbyssRecordDbService : ISpiralAbyssRecordDbService
{
    private readonly IServiceProvider serviceProvider;

    public async ValueTask<Dictionary<uint, SpiralAbyssEntry>> GetSpiralAbyssEntryListByUidAsync(string uid)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            List<SpiralAbyssEntry> entries = await appDbContext.SpiralAbysses
                    .Where(s => s.Uid == uid)
                    .OrderByDescending(s => s.ScheduleId)
                    .ToListAsync()
                    .ConfigureAwait(false);

            return entries.ToDictionary(e => e.ScheduleId);
        }
    }

    public async ValueTask UpdateSpiralAbyssEntryAsync(SpiralAbyssEntry entry)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.SpiralAbysses.UpdateAndSaveAsync(entry).ConfigureAwait(false);
        }
    }

    public async ValueTask AddSpiralAbyssEntryAsync(SpiralAbyssEntry entry)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.SpiralAbysses.AddAndSaveAsync(entry).ConfigureAwait(false);
        }
    }
}