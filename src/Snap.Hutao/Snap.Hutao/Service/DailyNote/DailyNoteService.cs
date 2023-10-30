// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Message;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Collections.ObjectModel;
using WebDailyNote = Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote.DailyNote;

namespace Snap.Hutao.Service.DailyNote;

/// <summary>
/// 实时便笺服务
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IDailyNoteService))]
internal sealed partial class DailyNoteService : IDailyNoteService, IRecipient<UserRemovedMessage>
{
    private readonly DailyNoteNotificationOperation dailyNoteNotificationOperation;
    private readonly IServiceProvider serviceProvider;
    private readonly IDailyNoteDbService dailyNoteDbService;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;

    private ObservableCollection<DailyNoteEntry>? entries;

    /// <inheritdoc/>
    public void Receive(UserRemovedMessage message)
    {
        // Database items have been deleted by cascade deleting.
        taskContext.InvokeOnMainThread(() => entries?.RemoveWhere(n => n.UserId == message.RemovedUserId));
    }

    /// <inheritdoc/>
    public async ValueTask AddDailyNoteAsync(UserAndUid userAndUid)
    {
        string roleUid = userAndUid.Uid.Value;

        if (!await dailyNoteDbService.ContainsUidAsync(roleUid).ConfigureAwait(false))
        {
            DailyNoteEntry newEntry = DailyNoteEntry.From(userAndUid);

            Web.Response.Response<WebDailyNote> dailyNoteResponse = await serviceProvider
                .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
                .Create(PlayerUid.IsOversea(roleUid))
                .GetDailyNoteAsync(userAndUid)
                .ConfigureAwait(false);

            if (dailyNoteResponse.IsOk())
            {
                newEntry.UpdateDailyNote(dailyNoteResponse.Data);
            }

            newEntry.UserGameRole = userService.GetUserGameRoleByUid(roleUid);
            await dailyNoteDbService.AddDailyNoteEntryAsync(newEntry).ConfigureAwait(false);

            newEntry.User = userAndUid.User;
            await taskContext.SwitchToMainThreadAsync();
            entries?.Add(newEntry);
        }
    }

    /// <inheritdoc/>
    public async ValueTask<ObservableCollection<DailyNoteEntry>> GetDailyNoteEntryCollectionAsync(bool forceRefresh = false)
    {
        if (entries is null)
        {
            // IUserService.GetUserGameRoleByUid only usable after call IUserService.GetRoleCollectionAsync
            await userService.GetRoleCollectionAsync().ConfigureAwait(false);
            await RefreshDailyNotesCoreAsync(forceRefresh).ConfigureAwait(false);

            List<DailyNoteEntry> entryList = await dailyNoteDbService.GetDailyNoteEntryIncludeUserListAsync().ConfigureAwait(false);
            entryList.ForEach(entry => { entry.UserGameRole = userService.GetUserGameRoleByUid(entry.Uid); });
            entries = new(entryList);
        }

        return entries;
    }

    /// <inheritdoc/>
    public ValueTask RefreshDailyNotesAsync()
    {
        return RefreshDailyNotesCoreAsync(true);
    }

    /// <inheritdoc/>
    public async ValueTask RemoveDailyNoteAsync(DailyNoteEntry entry)
    {
        await taskContext.SwitchToMainThreadAsync();
        ArgumentNullException.ThrowIfNull(entries);
        entries.Remove(entry);

        await taskContext.SwitchToBackgroundAsync();
        await dailyNoteDbService.DeleteDailyNoteEntryByIdAsync(entry.InnerId).ConfigureAwait(false);
    }

    public async ValueTask UpdateDailyNoteAsync(DailyNoteEntry entry)
    {
        await taskContext.SwitchToBackgroundAsync();
        await dailyNoteDbService.UpdateDailyNoteEntryAsync(entry).ConfigureAwait(false);
    }

    private async ValueTask RefreshDailyNotesCoreAsync(bool forceRefresh)
    {
        DailyNoteWebhookOperation dailyNoteWebhookOperation = serviceProvider.GetRequiredService<DailyNoteWebhookOperation>();

        foreach (DailyNoteEntry entry in await dailyNoteDbService.GetDailyNoteEntryIncludeUserListAsync().ConfigureAwait(false))
        {
            if (!forceRefresh && entry.DailyNote is not null)
            {
                continue;
            }

            Web.Response.Response<WebDailyNote> dailyNoteResponse = await serviceProvider
                .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
                .Create(PlayerUid.IsOversea(entry.Uid))
                .GetDailyNoteAsync(new(entry.User, entry.Uid))
                .ConfigureAwait(false);

            if (dailyNoteResponse.IsOk())
            {
                WebDailyNote dailyNote = dailyNoteResponse.Data;

                // 发送通知
                await dailyNoteNotificationOperation.SendAsync(entry).ConfigureAwait(false);

                // 集合内的实时便笺与数据库取出的非同一个对象，需要分别更新
                // cache
                await taskContext.SwitchToMainThreadAsync();
                if (entries?.SingleOrDefault(e => e.UserId == entry.UserId && e.Uid == entry.Uid) is { } cachedEntry)
                {
                    cachedEntry.UpdateDailyNote(dailyNote);
                    cachedEntry.ResinNotifySuppressed = entry.ResinNotifySuppressed;
                    cachedEntry.HomeCoinNotifySuppressed = entry.HomeCoinNotifySuppressed;
                    cachedEntry.TransformerNotifySuppressed = entry.TransformerNotifySuppressed;
                    cachedEntry.DailyTaskNotifySuppressed = entry.DailyTaskNotifySuppressed;
                    cachedEntry.ExpeditionNotifySuppressed = entry.ExpeditionNotifySuppressed;
                }

                // database
                entry.UpdateDailyNote(dailyNote);
                await dailyNoteDbService.UpdateDailyNoteEntryAsync(entry).ConfigureAwait(false);
                await dailyNoteWebhookOperation.TryPostDailyNoteToWebhookAsync(dailyNote).ConfigureAwait(false);
            }
        }
    }
}