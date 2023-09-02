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
            string message = SH.ServiceGachaLogArchiveCollectionUserdataCorruptedMessage.Format(ex.Message);
            throw ThrowHelper.UserdataCorrupted(message, ex);
        }
    }

    public List<GachaItem> GetGachaItemListByArchiveId(Guid archiveId)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return appDbContext.GachaItems
                .AsNoTracking()
                .Where(i => i.ArchiveId == archiveId)
                .OrderBy(i => i.Id)
                .ToList();
        }
    }

    public async ValueTask DeleteGachaArchiveByIdAsync(Guid archiveId)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.GachaArchives
                .ExecuteDeleteWhereAsync(a => a.InnerId == archiveId)
                .ConfigureAwait(false);
        }
    }

    public async ValueTask<long> GetNewestGachaItemIdByArchiveIdAndQueryTypeAsync(Guid archiveId, GachaConfigType queryType, CancellationToken token)
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

    public long GetNewestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaConfigType queryType)
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

    public long GetOldestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaConfigType queryType)
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

    public async ValueTask AddGachaArchiveAsync(GachaArchive archive)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.GachaArchives.AddAndSaveAsync(archive).ConfigureAwait(false);
        }
    }

    public void AddGachaArchive(GachaArchive archive)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.GachaArchives.AddAndSave(archive);
        }
    }

    public List<Web.Hutao.GachaLog.GachaItem> GetHutaoGachaItemList(Guid archiveId, GachaConfigType queryType, long endId)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return appDbContext.GachaItems
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
                .ToList();
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

    public void AddGachaItems(List<GachaItem> items)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.GachaItems.AddRangeAndSave(items);
        }
    }

    public void DeleteNewerGachaItemsByArchiveIdQueryTypeAndEndId(Guid archiveId, GachaConfigType queryType, long endId)
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
}