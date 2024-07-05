// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.DailyNote;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using System.Collections.ObjectModel;
using WebDailyNote = Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote.DailyNote;

namespace Snap.Hutao.Service.DailyNote;

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

    public void Receive(UserRemovedMessage message)
    {
        // Database items have been deleted by cascade deleting.
        taskContext.BeginInvokeOnMainThread(() => entries?.RemoveWhere(n => n.UserId == message.RemovedUser.InnerId));
    }

    public async ValueTask AddDailyNoteAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        string roleUid = userAndUid.Uid.Value;

        if (dailyNoteDbService.ContainsUid(roleUid))
        {
            return;
        }

        DailyNoteEntry newEntry = DailyNoteEntry.From(userAndUid);

        Web.Response.Response<WebDailyNote> dailyNoteResponse;
        DailyNoteMetadataContext context;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IGameRecordClient gameRecordClient = scope.ServiceProvider
                .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
                .Create(PlayerUid.IsOversea(roleUid));

            dailyNoteResponse = await gameRecordClient
                .GetDailyNoteAsync(userAndUid, token)
                .ConfigureAwait(false);

            context = await scope.GetRequiredService<IMetadataService>().GetContextAsync<DailyNoteMetadataContext>(token).ConfigureAwait(false);
        }

        if (dailyNoteResponse.IsOk())
        {
            newEntry.UpdateDailyNote(dailyNoteResponse.Data);
        }

        newEntry.UserGameRole = await userService.GetUserGameRoleByUidAsync(roleUid).ConfigureAwait(false);
        newEntry.ArchonQuestView = DailyNoteArchonQuestView.Create(newEntry.DailyNote, context.Chapters);
        dailyNoteDbService.AddDailyNoteEntry(newEntry);

        newEntry.User = userAndUid.User;
        await taskContext.SwitchToMainThreadAsync();
        entries?.Add(newEntry);
    }

    /// <inheritdoc/>
    public async ValueTask<ObservableCollection<DailyNoteEntry>> GetDailyNoteEntryCollectionAsync(bool forceRefresh = false, CancellationToken token = default)
    {
        if (entries is null)
        {
            await RefreshDailyNotesCoreAsync(forceRefresh, token).ConfigureAwait(false);

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                DailyNoteMetadataContext context = await scope.GetRequiredService<IMetadataService>().GetContextAsync<DailyNoteMetadataContext>(token).ConfigureAwait(false);

                List<DailyNoteEntry> entryList = dailyNoteDbService.GetDailyNoteEntryListIncludingUser();

                foreach (DailyNoteEntry entry in entryList)
                {
                    entry.UserGameRole = await userService.GetUserGameRoleByUidAsync(entry.Uid).ConfigureAwait(false);
                    entry.ArchonQuestView = DailyNoteArchonQuestView.Create(entry.DailyNote, context.Chapters);
                }

                entries = entryList.ToObservableCollection();
            }
        }

        return entries;
    }

    /// <inheritdoc/>
    public ValueTask RefreshDailyNotesAsync(CancellationToken token = default)
    {
        return RefreshDailyNotesCoreAsync(true, token);
    }

    /// <inheritdoc/>
    public async ValueTask RemoveDailyNoteAsync(DailyNoteEntry entry, CancellationToken token = default)
    {
        await taskContext.SwitchToMainThreadAsync();
        ArgumentNullException.ThrowIfNull(entries);
        entries.Remove(entry);

        await taskContext.SwitchToBackgroundAsync();
        dailyNoteDbService.DeleteDailyNoteEntryById(entry.InnerId);
    }

    public async ValueTask UpdateDailyNoteAsync(DailyNoteEntry entry, CancellationToken token = default)
    {
        await taskContext.SwitchToBackgroundAsync();
        dailyNoteDbService.UpdateDailyNoteEntry(entry);
    }

    private async ValueTask RefreshDailyNotesCoreAsync(bool forceRefresh, CancellationToken token = default)
    {
        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            DailyNoteWebhookOperation dailyNoteWebhookOperation = serviceProvider.GetRequiredService<DailyNoteWebhookOperation>();

            foreach (DailyNoteEntry entry in dailyNoteDbService.GetDailyNoteEntryListIncludingUser())
            {
                if (!forceRefresh && entry.DailyNote is not null)
                {
                    continue;
                }

                IGameRecordClient gameRecordClient = scope.ServiceProvider
                        .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
                        .Create(PlayerUid.IsOversea(entry.Uid));

                Web.Response.Response<WebDailyNote> dailyNoteResponse = await gameRecordClient
                    .GetDailyNoteAsync(new(entry.User, entry.Uid), token)
                    .ConfigureAwait(false);

                if (dailyNoteResponse.IsOk())
                {
                    WebDailyNote dailyNote = dailyNoteResponse.Data;
                    entry.UpdateDailyNote(dailyNote);

                    // 集合内的实时便笺与数据库取出的非同一个对象，需要分别更新
                    // Cache
                    await taskContext.SwitchToMainThreadAsync();
                    if (entries?.SingleOrDefault(e => e.UserId == entry.UserId && e.Uid == entry.Uid) is { } cachedEntry)
                    {
                        entry.CopyTo(cachedEntry);
                    }

                    // Database
                    {
                        // 发送通知必须早于数据库更新，否则会导致通知重复
                        await dailyNoteNotificationOperation.SendAsync(entry).ConfigureAwait(false);
                        dailyNoteDbService.UpdateDailyNoteEntry(entry);
                        dailyNoteWebhookOperation.TryPostDailyNoteToWebhook(entry.Uid, dailyNote);
                    }
                }
            }
        }
    }
}