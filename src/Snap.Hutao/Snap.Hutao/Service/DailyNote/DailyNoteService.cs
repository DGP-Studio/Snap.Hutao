// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Message;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using System.Collections.ObjectModel;
using WebDailyNote = Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote.DailyNote;

namespace Snap.Hutao.Service.DailyNote;

/// <summary>
/// 实时便笺服务
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton, typeof(IDailyNoteService))]
internal sealed class DailyNoteService : IDailyNoteService, IRecipient<UserRemovedMessage>
{
    private readonly IServiceProvider serviceProvider;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;
    private ObservableCollection<DailyNoteEntry>? entries;

    /// <summary>
    /// 构造一个新的实时便笺服务
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public DailyNoteService(IServiceProvider serviceProvider)
    {
        userService = serviceProvider.GetRequiredService<IUserService>();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        this.serviceProvider = serviceProvider;

        serviceProvider.GetRequiredService<IMessenger>().Register(this);
    }

    /// <inheritdoc/>
    public void Receive(UserRemovedMessage message)
    {
        // Database items have been deleted by cascade deleting.
        taskContext.InvokeOnMainThread(() => entries?.RemoveWhere(n => n.UserId == message.RemovedUserId));
    }

    /// <inheritdoc/>
    public async Task AddDailyNoteAsync(UserAndUid role)
    {
        string roleUid = role.Uid.Value;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!appDbContext.DailyNotes.Any(n => n.Uid == roleUid))
            {
                DailyNoteEntry newEntry = DailyNoteEntry.Create(role);

                Web.Response.Response<WebDailyNote> dailyNoteResponse = await scope.ServiceProvider
                    .PickRequiredService<IGameRecordClient>(PlayerUid.IsOversea(roleUid))
                    .GetDailyNoteAsync(role)
                    .ConfigureAwait(false);

                if (dailyNoteResponse.IsOk())
                {
                    newEntry.UpdateDailyNote(dailyNoteResponse.Data);
                }

                newEntry.UserGameRole = userService.GetUserGameRoleByUid(roleUid);
                await appDbContext.DailyNotes.AddAndSaveAsync(newEntry).ConfigureAwait(false);

                await taskContext.SwitchToMainThreadAsync();
                entries?.Add(newEntry);
            }
        }
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<DailyNoteEntry>> GetDailyNoteEntriesAsync()
    {
        if (entries == null)
        {
            await RefreshDailyNotesAsync().ConfigureAwait(false);

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                List<DailyNoteEntry> entryList = appDbContext.DailyNotes.ToList();
                entryList.ForEach(entry => { entry.UserGameRole = userService.GetUserGameRoleByUid(entry.Uid); });
                entries = new(entryList);
            }
        }

        return entries;
    }

    /// <inheritdoc/>
    public async ValueTask RefreshDailyNotesAsync()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            foreach (DailyNoteEntry entry in appDbContext.DailyNotes.Include(n => n.User))
            {
                Web.Response.Response<WebDailyNote> dailyNoteResponse = await scope.ServiceProvider
                    .PickRequiredService<IGameRecordClient>(PlayerUid.IsOversea(entry.Uid))
                    .GetDailyNoteAsync(new(entry.User, entry.Uid))
                    .ConfigureAwait(false);

                if (dailyNoteResponse.IsOk())
                {
                    WebDailyNote dailyNote = dailyNoteResponse.Data!;

                    // database
                    entry.UpdateDailyNote(dailyNote);

                    // cache
                    await taskContext.SwitchToMainThreadAsync();
                    entries?.SingleOrDefault(e => e.UserId == entry.UserId && e.Uid == entry.Uid)?.UpdateDailyNote(dailyNote);

                    await new DailyNoteNotifier(serviceProvider, entry).NotifyAsync().ConfigureAwait(false);
                    await appDbContext.DailyNotes.UpdateAndSaveAsync(entry).ConfigureAwait(false);
                }
                else
                {
                    IInfoBarService infoBarService = scope.ServiceProvider.GetRequiredService<IInfoBarService>();

                    // Special retcode handling for dailynote
                    if (dailyNoteResponse.ReturnCode == (int)Web.Response.KnownReturnCode.CODE1034)
                    {
                        infoBarService.Warning(dailyNoteResponse.ToString());
                    }
                    else
                    {
                        infoBarService.Error(dailyNoteResponse.ToString());
                    }
                }
            }
        }
    }

    /// <inheritdoc/>
    public async Task RemoveDailyNoteAsync(DailyNoteEntry entry)
    {
        await taskContext.SwitchToMainThreadAsync();
        entries!.Remove(entry);

        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.DailyNotes.ExecuteDeleteWhereAsync(d => d.InnerId == entry.InnerId).ConfigureAwait(false);
        }
    }
}