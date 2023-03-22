// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Message;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.User;
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
        ThreadHelper.InvokeOnMainThread(() =>
        {
            // Database items have been deleted by cascade deleting.
            entries?.RemoveWhere(n => n.UserId == message.RemovedUserId);
        });
    }

    /// <inheritdoc/>
    public async Task AddDailyNoteAsync(UserAndUid role)
    {
        string roleUid = role.Uid.Value;
        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            GameRecordClient gameRecordClient = scope.ServiceProvider.GetRequiredService<GameRecordClient>();
            GameRecordClientOs gameRecordClientOs = scope.ServiceProvider.GetRequiredService<GameRecordClientOs>();

            if (!appDbContext.DailyNotes.Any(n => n.Uid == roleUid))
            {
                DailyNoteEntry newEntry = DailyNoteEntry.Create(role);

                // 根据 Uid 的地区选择不同的 API
                Web.Response.Response<WebDailyNote> dailyNoteResponse;
                PlayerUid playerUid = new(roleUid);
                if (playerUid.Region == "cn_gf01" || playerUid.Region == "cn_qd01")
                {
                    dailyNoteResponse = await gameRecordClient
                    .GetDailyNoteAsync(role)
                    .ConfigureAwait(false);
                }
                else
                {
                    dailyNoteResponse = await gameRecordClientOs
                    .GetDailyNoteAsync(role)
                    .ConfigureAwait(false);
                }

                if (dailyNoteResponse.IsOk())
                {
                    newEntry.DailyNote = dailyNoteResponse.Data;
                }

                newEntry.UserGameRole = userService.GetUserGameRoleByUid(roleUid);
                await appDbContext.DailyNotes.AddAndSaveAsync(newEntry).ConfigureAwait(false);

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
                entries = new(entryList);
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
            GameRecordClientOs gameRecordClientOs = scope.ServiceProvider.GetRequiredService<GameRecordClientOs>();

            bool isSilentMode = appDbContext.Settings
                .SingleOrAdd(SettingEntry.DailyNoteSilentWhenPlayingGame, Core.StringLiterals.False)
                .GetBoolean();
            bool isGameRunning = scope.ServiceProvider.GetRequiredService<IGameService>().IsGameRunning();

            if (isSilentMode && isGameRunning)
            {
                // Prevent notify when we are in game && silent mode.
                notify = false;
            }

            foreach (DailyNoteEntry entry in appDbContext.DailyNotes.Include(n => n.User))
            {
                Web.Response.Response<WebDailyNote> dailyNoteResponse;
                PlayerUid playerUid = new(entry.Uid);
                if (playerUid.Region == "cn_gf01" || playerUid.Region == "cn_qd01")
                {
                    dailyNoteResponse = await gameRecordClient
                    .GetDailyNoteAsync(new(entry.User, entry.Uid))
                    .ConfigureAwait(false);
                }
                else
                {
                    dailyNoteResponse = await gameRecordClientOs
                    .GetDailyNoteAsync(new(entry.User, entry.Uid))
                    .ConfigureAwait(false);
                }

                if (dailyNoteResponse.ReturnCode == 0)
                {
                    WebDailyNote dailyNote = dailyNoteResponse.Data!;

                    // database
                    entry.DailyNote = dailyNote;

                    // cache
                    await ThreadHelper.SwitchToMainThreadAsync();
                    entries?.SingleOrDefault(e => e.UserId == entry.UserId && e.Uid == entry.Uid)?.UpdateDailyNote(dailyNote);

                    if (notify)
                    {
                        await new DailyNoteNotifier(scopeFactory, entry).NotifyAsync().ConfigureAwait(false);
                    }
                }
                else
                {
                    IInfoBarService infoBarService = scope.ServiceProvider.GetRequiredService<IInfoBarService>();

                    // special retcode handling for dailynote
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

            await appDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task RemoveDailyNoteAsync(DailyNoteEntry entry)
    {
        entries!.Remove(entry);

        using (IServiceScope scope = scopeFactory.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.DailyNotes.ExecuteDeleteWhereAsync(d => d.InnerId == entry.InnerId).ConfigureAwait(false);
        }
    }
}