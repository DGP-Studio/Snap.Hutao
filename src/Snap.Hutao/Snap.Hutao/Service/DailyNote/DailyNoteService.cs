// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.User;
using Snap.Hutao.ViewModel.DailyNote;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using WebDailyNote = Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.DailyNote.DailyNote;

namespace Snap.Hutao.Service.DailyNote;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IDailyNoteService))]
internal sealed partial class DailyNoteService : IDailyNoteService, IRecipient<UserRemovedMessage>
{
    private readonly DailyNoteNotificationOperation dailyNoteNotificationOperation;
    private readonly IDailyNoteRepository dailyNoteRepository;
    private readonly DailyNoteOptions dailyNoteOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;

    private readonly AsyncLock entriesLock = new();
    private ObservableCollection<DailyNoteEntry>? entries;

    public void Receive(UserRemovedMessage message)
    {
        // Database items have been deleted by cascade deleting.
        taskContext.BeginInvokeOnMainThread(() => entries?.RemoveWhere(n => n.UserId == message.RemovedUser.InnerId));
    }

    public async ValueTask AddDailyNoteAsync(DailyNoteMetadataContext context, UserAndUid userAndUid, CancellationToken token = default)
    {
        string roleUid = userAndUid.Uid.Value;

        if (dailyNoteRepository.ContainsUid(roleUid))
        {
            return;
        }

        DailyNoteEntry newEntry = DailyNoteEntry.From(userAndUid);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            Response<WebDailyNote> dailyNoteResponse = await GetDailyNoteAsync(scope, userAndUid, token).ConfigureAwait(false);
            if (ResponseValidator.TryValidate(dailyNoteResponse, serviceProvider, out WebDailyNote? data))
            {
                newEntry.Update(data);
            }
        }

        newEntry.UserGameRole = await userService.GetUserGameRoleByUidAsync(roleUid).ConfigureAwait(false);
        newEntry.ArchonQuestView = DailyNoteArchonQuestView.Create(newEntry.DailyNote, context.Chapters);
        dailyNoteRepository.AddDailyNoteEntry(newEntry);

        newEntry.User = userAndUid.User;
        await taskContext.SwitchToMainThreadAsync();
        entries?.Add(newEntry);
    }

    public async ValueTask<ObservableCollection<DailyNoteEntry>> GetDailyNoteEntryCollectionAsync(DailyNoteMetadataContext context, bool forceRefresh = false, CancellationToken token = default)
    {
        using (await entriesLock.LockAsync().ConfigureAwait(false))
        {
            if (entries is null)
            {
                await RefreshDailyNotesCoreAsync(forceRefresh, token).ConfigureAwait(false);
                ImmutableArray<DailyNoteEntry> entryList = dailyNoteRepository.GetDailyNoteEntryImmutableArrayIncludingUser();

                foreach (DailyNoteEntry entry in entryList)
                {
                    entry.UserGameRole = await userService.GetUserGameRoleByUidAsync(entry.Uid).ConfigureAwait(false);
                    entry.ArchonQuestView = DailyNoteArchonQuestView.Create(entry.DailyNote, context.Chapters);
                }

                entries = entryList.ToObservableCollection();
            }

            return entries;
        }
    }

    public ValueTask RefreshDailyNotesAsync(CancellationToken token = default)
    {
        return RefreshDailyNotesCoreAsync(true, token);
    }

    public async ValueTask RemoveDailyNoteAsync(DailyNoteEntry entry, CancellationToken token = default)
    {
        await taskContext.SwitchToMainThreadAsync();
        ArgumentNullException.ThrowIfNull(entries);
        entries.Remove(entry);

        await taskContext.SwitchToBackgroundAsync();
        dailyNoteRepository.DeleteDailyNoteEntryById(entry.InnerId);
    }

    public async ValueTask UpdateDailyNoteAsync(DailyNoteEntry entry, CancellationToken token = default)
    {
        await taskContext.SwitchToBackgroundAsync();
        dailyNoteRepository.UpdateDailyNoteEntry(entry);
    }

    private static async ValueTask<Response<WebDailyNote>> GetDailyNoteAsync(IServiceScope scope, UserAndUid userAndUid, CancellationToken token = default)
    {
        IGameRecordClient gameRecordClient = scope.ServiceProvider
            .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
            .Create(userAndUid.IsOversea);

        return await gameRecordClient.GetDailyNoteAsync(userAndUid, token).ConfigureAwait(false);
    }

    private async ValueTask RefreshDailyNotesCoreAsync(bool forceRefresh, CancellationToken token = default)
    {
        await taskContext.SwitchToBackgroundAsync();

        bool autoRefresh = dailyNoteOptions.IsAutoRefreshEnabled;
        TimeSpan threshold = TimeSpan.FromSeconds(dailyNoteOptions.SelectedRefreshTime?.Value ?? 60 * 60 * 4);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            DailyNoteWebhookOperation dailyNoteWebhookOperation = serviceProvider.GetRequiredService<DailyNoteWebhookOperation>();

            foreach (DailyNoteEntry dbEntry in dailyNoteRepository.GetDailyNoteEntryImmutableArrayIncludingUser())
            {
                if (!(forceRefresh || (autoRefresh && dbEntry.RefreshTime < DateTimeOffset.Now - threshold)))
                {
                    continue;
                }

                Response<WebDailyNote> dailyNoteResponse = await GetDailyNoteAsync(scope, UserAndUid.From(dbEntry.User, dbEntry.Uid), token).ConfigureAwait(false);
                if (ResponseValidator.TryValidate(dailyNoteResponse, serviceProvider, out WebDailyNote? dailyNote))
                {
                    await taskContext.SwitchToMainThreadAsync();
                    if (entries?.SingleOrDefault(e => e.UserId == dbEntry.UserId && e.Uid == dbEntry.Uid) is not { } cachedEntry)
                    {
                        // This can only happen when the entry is removing from the collection.
                        // And the entry is not removed from the database yet. We just skip it.
                        continue;
                    }

                    dbEntry.Update(dailyNote);

                    // The dbEntry will be updated before sending notification (Check suppression).
                    await dailyNoteNotificationOperation.SendAsync(dbEntry).ConfigureAwait(false);
                    dailyNoteRepository.UpdateDailyNoteEntry(dbEntry);

                    dailyNoteWebhookOperation.TryPostDailyNoteToWebhook(dbEntry.Uid, dailyNote);

                    // After everything is done, we copy the updated entry to the cached entry.
                    dbEntry.CopyTo(cachedEntry);
                }
            }
        }
    }
}