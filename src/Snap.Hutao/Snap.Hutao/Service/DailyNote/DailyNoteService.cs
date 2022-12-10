// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Extension;
using Snap.Hutao.Message;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote;
using System.Collections.ObjectModel;
using WebDailyNote = Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote.DailyNote;

namespace Snap.Hutao.Service.DailyNote;

/// <summary>
/// 实时便笺服务
/// </summary>
[Injection(InjectAs.Singleton, typeof(IDailyNoteService))]
internal class DailyNoteService : IDailyNoteService, IRecipient<UserRemovedMessage>
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IUserService userService;
    private ObservableCollection<DailyNoteEntry>? entries;

    /// <summary>
    /// 构造一个新的实时便笺服务
    /// </summary>
    /// <param name="scopeFactory">范围工厂</param>
    /// <param name="userService">用户服务</param>
    /// <param name="messenger">消息器</param>
    public DailyNoteService(IServiceScopeFactory scopeFactory, IUserService userService, IMessenger messenger)
    {
        this.scopeFactory = scopeFactory;
        this.userService = userService;

        messenger.Register(this);
    }

    /// <inheritdoc/>
    public void Receive(UserRemovedMessage message)
    {
        entries?.RemoveWhere(n => n.UserId == message.RemovedUserId);
    }

    /// <inheritdoc/>
    public async Task AddDailyNoteAsync(UserAndRole role)
    {
        string roleUid = role.Role.GameUid;
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            GameRecordClient gameRecordClient = scope.ServiceProvider.GetRequiredService<GameRecordClient>();

            if (!appDbContext.DailyNotes.Any(n => n.Uid == roleUid))
            {
                DailyNoteEntry newEntry = DailyNoteEntry.Create(role);
                newEntry.DailyNote = await gameRecordClient.GetDailyNoteAsync(role.User, newEntry.Uid).ConfigureAwait(false);
                appDbContext.DailyNotes.AddAndSave(newEntry);

                newEntry.UserGameRole = userService.GetUserGameRoleByUid(roleUid);
                await ThreadHelper.SwitchToMainThreadAsync();
                entries?.Add(newEntry);
            }
        }
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<DailyNoteEntry>> GetDailyNoteEntriesAsync()
    {
        if (entries == null)
        {
            await RefreshDailyNotesAsync(false).ConfigureAwait(false);

            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                List<DailyNoteEntry> entryList = appDbContext.DailyNotes.AsNoTracking().ToList();
                entryList.ForEach(entry => { entry.UserGameRole = userService.GetUserGameRoleByUid(entry.Uid); });
                entries = new(appDbContext.DailyNotes);
            }
        }

        return entries;
    }

    /// <inheritdoc/>
    public async ValueTask RefreshDailyNotesAsync(bool notify)
    {
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            GameRecordClient gameRecordClient = scope.ServiceProvider.GetRequiredService<GameRecordClient>();
            BindingClient bindingClient = scope.ServiceProvider.GetRequiredService<BindingClient>();

            foreach (DailyNoteEntry entry in appDbContext.DailyNotes.Include(n => n.User))
            {
                WebDailyNote? dailyNote = await gameRecordClient.GetDailyNoteAsync(entry.User, entry.Uid).ConfigureAwait(false);

                // database
                entry.DailyNote = dailyNote;

                // cache
                await ThreadHelper.SwitchToMainThreadAsync();
                entries?.SingleOrDefault(e => e.UserId == entry.UserId && e.Uid == entry.Uid)?.UpdateDailyNote(dailyNote);

                if (notify)
                {
                    await new DailyNoteNotifier(scopeFactory, bindingClient, entry).NotifyAsync().ConfigureAwait(false);
                }
            }

            await appDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public void RemoveDailyNote(DailyNoteEntry entry)
    {
        entries!.Remove(entry);

        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<AppDbContext>().DailyNotes.RemoveAndSave(entry);
        }
    }
}