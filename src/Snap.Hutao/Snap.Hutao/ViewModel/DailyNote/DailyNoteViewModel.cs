// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service;
using Snap.Hutao.Service.DailyNote;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.Control;
using Snap.Hutao.UI.Xaml.View.Dialog;
using Snap.Hutao.UI.Xaml.View.Window.WebView2;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.DailyNote;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class DailyNoteViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IDailyNoteService dailyNoteService;
    private readonly DailyNoteOptions dailyNoteOptions;
    private readonly IMetadataService metadataService;
    private readonly IInfoBarService infoBarService;
    private readonly RuntimeOptions runtimeOptions;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly AppOptions appOptions;

    private AdvancedDbCollectionView<User.User, Model.Entity.User>? users;
    private ObservableCollection<DailyNoteEntry>? dailyNoteEntries;

    public DailyNoteOptions DailyNoteOptions { get => dailyNoteOptions; }

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    public AppOptions AppOptions { get => appOptions; }

    public IJSBridgeUriSourceProvider VerifyUrlSource { get; } = new DailyJSBridgeUriSourceProvider();

    public AdvancedDbCollectionView<User.User, Model.Entity.User>? Users { get => users; set => SetProperty(ref users, value); }

    public ObservableCollection<DailyNoteEntry>? DailyNoteEntries { get => dailyNoteEntries; set => SetProperty(ref dailyNoteEntries, value); }

    protected override async ValueTask<bool> InitializeOverrideAsync()
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            try
            {
                await taskContext.SwitchToBackgroundAsync();
                AdvancedDbCollectionView<User.User, Model.Entity.User> users = await userService.GetUsersAsync().ConfigureAwait(false);
                ObservableCollection<DailyNoteEntry> entries = await dailyNoteService.GetDailyNoteEntryCollectionAsync().ConfigureAwait(false);

                await taskContext.SwitchToMainThreadAsync();
                Users = users;
                DailyNoteEntries = entries;
                return true;
            }
            catch (HutaoException ex)
            {
                infoBarService.Error(ex);
            }
        }

        return false;
    }

    [Command("TrackCurrentUserAndUidCommand")]
    private async Task TrackCurrentUserAndUidAsync()
    {
        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
            return;
        }

        ContentDialog dialog = await contentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelDailyNoteRequestProgressTitle)
            .ConfigureAwait(false);

        using (await dialog.BlockAsync(contentDialogFactory).ConfigureAwait(false))
        {
            await dailyNoteService.AddDailyNoteAsync(userAndUid).ConfigureAwait(false);
        }
    }

    [Command("RefreshCommand")]
    private async Task RefreshAsync()
    {
        await dailyNoteService.RefreshDailyNotesAsync().ConfigureAwait(false);
    }

    [Command("RemoveDailyNoteCommand")]
    private async Task RemoveDailyNoteAsync(DailyNoteEntry? entry)
    {
        if (entry is not null)
        {
            await dailyNoteService.RemoveDailyNoteAsync(entry).ConfigureAwait(false);
        }
    }

    [Command("ModifyNotificationCommand")]
    private async Task ModifyDailyNoteNotificationAsync(DailyNoteEntry? entry)
    {
        if (entry is not null)
        {
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                // ContentDialog must be created by main thread.
                await taskContext.SwitchToMainThreadAsync();
                DailyNoteNotificationDialog dialog = await contentDialogFactory.CreateInstanceAsync<DailyNoteNotificationDialog>(entry).ConfigureAwait(true);
                await dialog.ShowAsync();

                await taskContext.SwitchToBackgroundAsync();
                await dailyNoteService.UpdateDailyNoteAsync(entry).ConfigureAwait(false);
            }
        }
    }

    [Command("ConfigDailyNoteWebhookUrlCommand")]
    private async Task ConfigDailyNoteWebhookUrlAsync()
    {
        DailyNoteWebhookDialog dialog = await contentDialogFactory.CreateInstanceAsync<DailyNoteWebhookDialog>().ConfigureAwait(true);
        dialog.Text = dailyNoteOptions.WebhookUrl;
        (bool isOk, string url) = await dialog.GetInputUrlAsync().ConfigureAwait(false);

        if (isOk)
        {
            await taskContext.SwitchToMainThreadAsync();
            dailyNoteOptions.WebhookUrl = url;
            infoBarService.Information(SH.ViewModelDailyNoteConfigWebhookUrlComplete);
        }
    }
}