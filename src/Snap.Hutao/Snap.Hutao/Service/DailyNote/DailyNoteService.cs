// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Extension;
using Snap.Hutao.Message;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.User;
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
                newEntry.DailyNote = await gameRecordClient.GetDialyNoteAsync(role.User, newEntry.Uid).ConfigureAwait(false);
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
                List<DailyNoteEntry> entryList = appDbContext.DailyNotes.ToList();
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

            foreach (DailyNoteEntry entry in appDbContext.DailyNotes.Include(n => n.User))
            {
                WebDailyNote? dailyNote = await gameRecordClient.GetDialyNoteAsync(entry.User, entry.Uid).ConfigureAwait(false);

                // database
                entry.DailyNote = dailyNote;

                // cache
                Guid userId = entry.UserId;
                await ThreadHelper.SwitchToMainThreadAsync();
                entries?.Single(e => e.UserId == userId).UpdateDailyNote(dailyNote);

                if (notify)
                {
                    await NotifyDailyNoteAsync(entry).ConfigureAwait(false);
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
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.DailyNotes.RemoveAndSave(entry);
        }
    }

    private async ValueTask NotifyDailyNoteAsync(DailyNoteEntry entry)
    {
        if (entry.DailyNote == null)
        {
            return;
        }

        List<string> hints = new();
        if (!entry.ResinNotifySuppressed && entry.DailyNote.CurrentResin >= entry.ResinNotifyThreshold)
        {
            hints.Add($"当前原粹树脂：{entry.DailyNote.CurrentResin}");
            entry.ResinNotifySuppressed = true;
        }

        if (!entry.HomeCoinNotifySuppressed && entry.DailyNote.CurrentHomeCoin >= entry.HomeCoinNotifyThreshold)
        {
            hints.Add($"当前洞天宝钱：{entry.DailyNote.CurrentHomeCoin}");
            entry.HomeCoinNotifySuppressed = true;
        }

        if (!entry.DailyTaskNotifySuppressed && entry.DailyTaskNotify && !entry.DailyNote.IsExtraTaskRewardReceived)
        {
            hints.Add(entry.DailyNote.ExtraTaskRewardDescription);
            entry.DailyTaskNotifySuppressed = true;
        }

        if (!entry.TransformerNotifySuppressed && entry.TransformerNotify && entry.DailyNote.Transformer.Obtained && entry.DailyNote.Transformer.RecoveryTime.Reached)
        {
            hints.Add("参量质变仪已准备完成");
            entry.TransformerNotifySuppressed = true;
        }

        if (!entry.ExpeditionNotifySuppressed && entry.ExpeditionNotify && entry.DailyNote.Expeditions.All(e => e.Status == ExpeditionStatus.Finished))
        {
            hints.Add("探索派遣已完成");
            entry.ExpeditionNotifySuppressed = true;
        }
    }
}