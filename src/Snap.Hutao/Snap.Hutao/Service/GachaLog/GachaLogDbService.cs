// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGachaLogDbService))]
internal sealed partial class GachaLogDbService : IGachaLogDbService
{
    private readonly IServiceProvider serviceProvider;

    public ObservableCollection<GachaArchive> GetGachaArchiveCollection()
    {
        try
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                return appDbContext.GachaArchives.AsNoTracking().ToObservableCollection();
            }
        }
        catch (SqliteException ex)
        {
            string message = SH.FormatServiceGachaLogArchiveCollectionUserdataCorruptedMessage(ex.Message);
            throw ThrowHelper.UserdataCorrupted(message, ex);
        }
    }

    public List<GachaItem> GetGachaItemListByArchiveId(Guid archiveId)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            IOrderedQueryable<GachaItem> result = appDbContext.GachaItems
                .AsNoTracking()
                .Where(i => i.ArchiveId == archiveId)
                .OrderBy(i => i.Id);
            return [.. result];
        }
    }

    public async ValueTask<List<GachaItem>> GetGachaItemListByArchiveIdAsync(Guid archiveId)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await appDbContext.GachaItems
                .AsNoTracking()
                .Where(i => i.ArchiveId == archiveId)
                .OrderBy(i => i.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }

    public async ValueTask RemoveGachaArchiveByIdAsync(Guid archiveId)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.GachaArchives
                .ExecuteDeleteWhereAsync(a => a.InnerId == archiveId)
                .ConfigureAwait(false);
        }
    }

    public async ValueTask<long> GetNewestGachaItemIdByArchiveIdAndQueryTypeAsync(Guid archiveId, GachaType queryType, CancellationToken token)
    {
        GachaItem? item = null;

        try
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // TODO: replace with MaxBy
                // https://github.com/dotnet/efcore/issues/25566
                // .MaxBy(i => i.Id);
                item = await appDbContext.GachaItems
                    .AsNoTracking()
                    .Where(i => i.ArchiveId == archiveId)
                    .Where(i => i.QueryType == queryType)
                    .OrderByDescending(i => i.Id)
                    .FirstOrDefaultAsync(token)
                    .ConfigureAwait(false);
            }
        }
        catch (SqliteException ex)
        {
            ThrowHelper.UserdataCorrupted(SH.ServiceGachaLogEndIdUserdataCorruptedMessage, ex);
        }

        return item?.Id ?? 0L;
    }

    public long GetNewestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaType queryType)
    {
        GachaItem? item = null;

        try
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // TODO: replace with MaxBy
                // https://github.com/dotnet/efcore/issues/25566
                // .MaxBy(i => i.Id);
                item = appDbContext.GachaItems
                    .AsNoTracking()
                    .Where(i => i.ArchiveId == archiveId)
                    .Where(i => i.QueryType == queryType)
                    .OrderByDescending(i => i.Id)
                    .FirstOrDefault();
            }
        }
        catch (SqliteException ex)
        {
            ThrowHelper.UserdataCorrupted(SH.ServiceGachaLogEndIdUserdataCorruptedMessage, ex);
        }

        return item?.Id ?? 0L;
    }

    public async ValueTask<long> GetNewestGachaItemIdByArchiveIdAndQueryTypeAsync(Guid archiveId, GachaType queryType)
    {
        GachaItem? item = null;

        try
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // TODO: replace with MaxBy
                // https://github.com/dotnet/efcore/issues/25566
                // .MaxBy(i => i.Id);
                item = await appDbContext.GachaItems
                    .AsNoTracking()
                    .Where(i => i.ArchiveId == archiveId)
                    .Where(i => i.QueryType == queryType)
                    .OrderByDescending(i => i.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
            }
        }
        catch (SqliteException ex)
        {
            ThrowHelper.UserdataCorrupted(SH.ServiceGachaLogEndIdUserdataCorruptedMessage, ex);
        }

        return item?.Id ?? 0L;
    }

    public long GetOldestGachaItemIdByArchiveId(Guid archiveId)
    {
        GachaItem? item = null;

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // TODO: replace with MaxBy
            // https://github.com/dotnet/efcore/issues/25566
            // .MaxBy(i => i.Id);
            item = appDbContext.GachaItems
                .AsNoTracking()
                .Where(i => i.ArchiveId == archiveId)
                .OrderBy(i => i.Id)
                .FirstOrDefault();
        }

        return item?.Id ?? long.MaxValue;
    }

    public async ValueTask<long> GetOldestGachaItemIdByArchiveIdAsync(Guid archiveId)
    {
        GachaItem? item = null;

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // TODO: replace with MaxBy
            // https://github.com/dotnet/efcore/issues/25566
            // .MaxBy(i => i.Id);
            item = await appDbContext.GachaItems
                .AsNoTracking()
                .Where(i => i.ArchiveId == archiveId)
                .OrderBy(i => i.Id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }

        return item?.Id ?? long.MaxValue;
    }

    public long GetOldestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaType queryType)
    {
        GachaItem? item = null;

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // TODO: replace with MaxBy
            // https://github.com/dotnet/efcore/issues/25566
            // .MaxBy(i => i.Id);
            item = appDbContext.GachaItems
                .AsNoTracking()
                .Where(i => i.ArchiveId == archiveId && i.QueryType == queryType)
                .OrderBy(i => i.Id)
                .FirstOrDefault();
        }

        return item?.Id ?? long.MaxValue;
    }

    public async ValueTask<long> GetOldestGachaItemIdByArchiveIdAndQueryTypeAsync(Guid archiveId, GachaType queryType, CancellationToken token)
    {
        GachaItem? item = null;

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // TODO: replace with MaxBy
            // https://github.com/dotnet/efcore/issues/25566
            // .MaxBy(i => i.Id);
            item = await appDbContext.GachaItems
                .AsNoTracking()
                .Where(i => i.ArchiveId == archiveId && i.QueryType == queryType)
                .OrderBy(i => i.Id)
                .FirstOrDefaultAsync(token)
                .ConfigureAwait(false);
        }

        return item?.Id ?? long.MaxValue;
    }

    public void AddGachaArchive(GachaArchive archive)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.GachaArchives.AddAndSave(archive);
        }
    }

    public async ValueTask AddGachaArchiveAsync(GachaArchive archive)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.GachaArchives.AddAndSaveAsync(archive).ConfigureAwait(false);
        }
    }

    public List<Web.Hutao.GachaLog.GachaItem> GetHutaoGachaItemList(Guid archiveId, GachaType queryType, long endId)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            IQueryable<Web.Hutao.GachaLog.GachaItem> result = appDbContext.GachaItems
                .AsNoTracking()
                .Where(i => i.ArchiveId == archiveId)
                .Where(i => i.QueryType == queryType)
                .OrderByDescending(i => i.Id)
                .Where(i => i.Id > endId)

                // Keep this to make SQL generates correctly
                .Select(i => new Web.Hutao.GachaLog.GachaItem()
                {
                    GachaType = i.GachaType,
                    QueryType = i.QueryType,
                    ItemId = i.ItemId,
                    Time = i.Time,
                    Id = i.Id,
                });
            return [.. result];
        }
    }

    public async ValueTask<List<Web.Hutao.GachaLog.GachaItem>> GetHutaoGachaItemListAsync(Guid archiveId, GachaType queryType, long endId)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await appDbContext.GachaItems
                .AsNoTracking()
                .Where(i => i.ArchiveId == archiveId)
                .Where(i => i.QueryType == queryType)
                .OrderByDescending(i => i.Id)
                .Where(i => i.Id > endId)

                // Keep this to make SQL generates correctly
                .Select(i => new Web.Hutao.GachaLog.GachaItem()
                {
                    GachaType = i.GachaType,
                    QueryType = i.QueryType,
                    ItemId = i.ItemId,
                    Time = i.Time,
                    Id = i.Id,
                })
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }

    public async ValueTask<GachaArchive?> GetGachaArchiveByIdAsync(Guid archiveId, CancellationToken token)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await appDbContext.GachaArchives
                .AsNoTracking()
                .SingleOrDefaultAsync(a => a.InnerId == archiveId, token)
                .ConfigureAwait(false);
        }
    }

    public async ValueTask<GachaArchive?> GetGachaArchiveByUidAsync(string uid, CancellationToken token)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await appDbContext.GachaArchives
                .AsNoTracking()
                .SingleOrDefaultAsync(a => a.Uid == uid, token)
                .ConfigureAwait(false);
        }
    }

    public async ValueTask AddGachaItemsAsync(List<GachaItem> items)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.GachaItems.AddRangeAndSaveAsync(items).ConfigureAwait(false);
        }
    }

    public void AddGachaItemRange(List<GachaItem> items)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.GachaItems.AddRangeAndSave(items);
        }
    }

    public async ValueTask AddGachaItemRangeAsync(List<GachaItem> items)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.GachaItems.AddRangeAndSaveAsync(items).ConfigureAwait(false);
        }
    }

    public void RemoveNewerGachaItemRangeByArchiveIdQueryTypeAndEndId(Guid archiveId, GachaType queryType, long endId)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.GachaItems
                .AsNoTracking()
                .Where(i => i.ArchiveId == archiveId && i.QueryType == queryType)
                .Where(i => i.Id >= endId)
                .ExecuteDelete();
        }
    }

    public async ValueTask RemoveNewerGachaItemRangeByArchiveIdQueryTypeAndEndIdAsync(Guid archiveId, GachaType queryType, long endId)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.GachaItems
                .AsNoTracking()
                .Where(i => i.ArchiveId == archiveId && i.QueryType == queryType)
                .Where(i => i.Id >= endId)
                .ExecuteDeleteAsync()
                .ConfigureAwait(false);
        }
    }
}