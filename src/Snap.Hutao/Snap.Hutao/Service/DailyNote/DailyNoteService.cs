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
using Snap.Hutao.Web.Response;
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

    private ObservableCollection<DailyNoteEntry>? entries;

    public void Receive(UserRemovedMessage message)
    {
        // Database items have been deleted by cascade deleting.
        taskContext.BeginInvokeOnMainThread(() => entries?.RemoveWhere(n => n.UserId == message.RemovedUser.InnerId));
    }

    public async ValueTask AddDailyNoteAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        string roleUid = userAndUid.Uid.Value;

        if (dailyNoteRepository.ContainsUid(roleUid))
        {
            return;
        }

        DailyNoteEntry newEntry = DailyNoteEntry.From(userAndUid);

        Response<WebDailyNote> dailyNoteResponse;
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

        if (ResponseValidator.TryValidate(dailyNoteResponse, serviceProvider, out WebDailyNote? data))
        {
            newEntry.UpdateDailyNote(data);
        }

        newEntry.UserGameRole = await userService.GetUserGameRoleByUidAsync(roleUid).ConfigureAwait(false);
        newEntry.ArchonQuestView = DailyNoteArchonQuestView.Create(newEntry.DailyNote, context.Chapters);
        dailyNoteRepository.AddDailyNoteEntry(newEntry);

        newEntry.User = userAndUid.User;
        await taskContext.SwitchToMainThreadAsync();
        entries?.Add(newEntry);
    }

    public async ValueTask<ObservableCollection<DailyNoteEntry>> GetDailyNoteEntryCollectionAsync(bool forceRefresh = false, CancellationToken token = default)
    {
        if (entries is null)
        {
            await RefreshDailyNotesCoreAsync(forceRefresh, token).ConfigureAwait(false);

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                DailyNoteMetadataContext context = await scope.GetRequiredService<IMetadataService>().GetContextAsync<DailyNoteMetadataContext>(token).ConfigureAwait(false);

                List<DailyNoteEntry> entryList = dailyNoteRepository.GetDailyNoteEntryListIncludingUser();

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

    private async ValueTask RefreshDailyNotesCoreAsync(bool forceRefresh, CancellationToken token = default)
    {
        await taskContext.SwitchToBackgroundAsync();

        bool autoRefresh = dailyNoteOptions.IsAutoRefreshEnabled;
        TimeSpan threshold = TimeSpan.FromSeconds(dailyNoteOptions.SelectedRefreshTime?.Value ?? 60 * 60 * 4);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            DailyNoteWebhookOperation dailyNoteWebhookOperation = serviceProvider.GetRequiredService<DailyNoteWebhookOperation>();

            foreach (DailyNoteEntry entry in dailyNoteRepository.GetDailyNoteEntryListIncludingUser())
            {
                if (!(forceRefresh || (autoRefresh && entry.RefreshTime < DateTimeOffset.Now - threshold)))
                {
                    continue;
                }

                IGameRecordClient gameRecordClient = scope.ServiceProvider
                    .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
                    .Create(PlayerUid.IsOversea(entry.Uid));

                Response<WebDailyNote> dailyNoteResponse = await gameRecordClient
                    .GetDailyNoteAsync(new(entry.User, entry.Uid), token)
                    .ConfigureAwait(false);

                if (ResponseValidator.TryValidate(dailyNoteResponse, serviceProvider, out WebDailyNote? dailyNote))
                {
                    entry.UpdateDailyNote(dailyNote);

                    // 集合内的实时便笺与数据库取出的非同一个对象，需要分别更新
                    // Cache
                    await taskContext.SwitchToMainThreadAsync();
                    if (entries?.SingleOrDefault(e => e.UserId == entry.UserId && e.Uid == entry.Uid) is { } cachedEntry)
                    {
                        entry.CopyTo(cachedEntry);
                    }

                    // 发送通知必须早于数据库更新，否则会导致通知重复
                    await dailyNoteNotificationOperation.SendAsync(entry).ConfigureAwait(false);
                    dailyNoteRepository.UpdateDailyNoteEntry(entry);
                    dailyNoteWebhookOperation.TryPostDailyNoteToWebhook(entry.Uid, dailyNote);
                }
            }
        }
    }
}