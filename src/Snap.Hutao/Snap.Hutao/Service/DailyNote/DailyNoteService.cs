// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Message;
using Snap.Hutao.Model.Entity;
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
        taskContext.BeginInvokeOnMainThread(() => entries?.RemoveWhere(n => n.UserId == message.RemovedUserId));
    }

    /// <inheritdoc/>
    public async ValueTask AddDailyNoteAsync(UserAndUid userAndUid)
    {
        string roleUid = userAndUid.Uid.Value;

        if (await dailyNoteDbService.ContainsUidAsync(roleUid).ConfigureAwait(false))
        {
            return;
        }

        DailyNoteEntry newEntry = DailyNoteEntry.From(userAndUid);

        Web.Response.Response<WebDailyNote> dailyNoteResponse;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IGameRecordClient gameRecordClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
                .Create(PlayerUid.IsOversea(roleUid));

            dailyNoteResponse = await gameRecordClient
                .GetDailyNoteAsync(userAndUid)
                .ConfigureAwait(false);
        }

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

    /// <inheritdoc/>
    public async ValueTask<ObservableCollection<DailyNoteEntry>> GetDailyNoteEntryCollectionAsync(bool forceRefresh = false)
    {
        if (entries is null)
        {
            // IUserService.GetUserGameRoleByUid only usable after call IUserService.GetRoleCollectionAsync
            await userService.GetRoleCollectionAsync().ConfigureAwait(false);
            await RefreshDailyNotesCoreAsync(forceRefresh).ConfigureAwait(false);

            List<DailyNoteEntry> entryList = await dailyNoteDbService.GetDailyNoteEntryListIncludingUserAsync().ConfigureAwait(false);
            entryList.ForEach(entry => { entry.UserGameRole = userService.GetUserGameRoleByUid(entry.Uid); });
            entries = entryList.ToObservableCollection();
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
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            DailyNoteWebhookOperation dailyNoteWebhookOperation = serviceProvider.GetRequiredService<DailyNoteWebhookOperation>();

            foreach (DailyNoteEntry entry in await dailyNoteDbService.GetDailyNoteEntryListIncludingUserAsync().ConfigureAwait(false))
            {
                if (!forceRefresh && entry.DailyNote is not null)
                {
                    continue;
                }

                IGameRecordClient gameRecordClient = scope.ServiceProvider
                        .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
                        .Create(PlayerUid.IsOversea(entry.Uid));

                Web.Response.Response<WebDailyNote> dailyNoteResponse = await gameRecordClient
                    .GetDailyNoteAsync(new(entry.User, entry.Uid))
                    .ConfigureAwait(false);

                if (dailyNoteResponse.IsOk())
                {
                    WebDailyNote dailyNote = dailyNoteResponse.Data;
                    entry.UpdateDailyNote(dailyNote);

                    // 集合内的实时便笺与数据库取出的非同一个对象，需要分别更新
                    // cache
                    await taskContext.SwitchToMainThreadAsync();
                    if (entries?.SingleOrDefault(e => e.UserId == entry.UserId && e.Uid == entry.Uid) is { } cachedEntry)
                    {
                        entry.CopyTo(cachedEntry);
                    }

                    // database
                    {
                        // 发送通知必须早于数据库更新，否则会导致通知重复
                        await dailyNoteNotificationOperation.SendAsync(entry).ConfigureAwait(false);
                        await dailyNoteDbService.UpdateDailyNoteEntryAsync(entry).ConfigureAwait(false);
                        await dailyNoteWebhookOperation.TryPostDailyNoteToWebhookAsync(entry.Uid, dailyNote).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}