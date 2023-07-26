// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Message;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using System.Collections.ObjectModel;
using WebDailyNote = Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote.DailyNote;

namespace Snap.Hutao.Service.DailyNote;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IDailyNoteDbService))]
internal sealed partial class DailyNoteDbService : IDailyNoteDbService
{
    private readonly IServiceProvider serviceProvider;

    public bool ContainsUid(string uid)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return appDbContext.DailyNotes.AsNoTracking().Any(n => n.Uid == uid);
        }
    }

    public async ValueTask AddDailyNoteEntryAsync(DailyNoteEntry entry)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.DailyNotes.AddAndSaveAsync(entry).ConfigureAwait(false);
        }
    }

    public async ValueTask DeleteDailyNoteEntryByIdAsync(Guid entryId)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.DailyNotes.ExecuteDeleteWhereAsync(d => d.InnerId == entryId).ConfigureAwait(false);
        }
    }

    public async ValueTask UpdateDailyNoteEntryAsync(DailyNoteEntry entry)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.DailyNotes.UpdateAndSaveAsync(entry).ConfigureAwait(false);
        }
    }

    public List<DailyNoteEntry> GetDailyNoteEntryList()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return appDbContext.DailyNotes.AsNoTracking().ToList();
        }
    }

    public List<DailyNoteEntry> GetDailyNoteEntryIncludeUserList()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return appDbContext.DailyNotes.AsNoTracking().Include(n => n.User).ToList();
        }
    }
}