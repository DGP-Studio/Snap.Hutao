// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Core;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.DailyNote;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Control;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.ViewModel.User;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.DailyNote;

/// <summary>
/// 实时便笺视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class DailyNoteViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IDailyNoteService dailyNoteService;
    private readonly IInfoBarService infoBarService;
    private readonly DailyNoteOptions options;
    private readonly RuntimeOptions runtimeOptions;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;

    private ObservableCollection<UserAndUid>? userAndUids;
    private ObservableCollection<DailyNoteEntry>? dailyNoteEntries;

    /// <summary>
    /// 选项
    /// </summary>
    public DailyNoteOptions Options { get => options; }

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    [SuppressMessage("", "CA1822")]
    public IWebViewerSource VerifyUrlSource { get => new DailyNoteWebViewerSource(); }

    /// <summary>
    /// 用户与角色集合
    /// </summary>
    public ObservableCollection<UserAndUid>? UserAndUids { get => userAndUids; set => SetProperty(ref userAndUids, value); }

    /// <summary>
    /// 实时便笺集合
    /// </summary>
    public ObservableCollection<DailyNoteEntry>? DailyNoteEntries { get => dailyNoteEntries; set => SetProperty(ref dailyNoteEntries, value); }

    protected override async ValueTask<bool> InitializeUIAsync()
    {
        try
        {
            await taskContext.SwitchToBackgroundAsync();
            ObservableCollection<UserAndUid> roles = await userService.GetRoleCollectionAsync().ConfigureAwait(false);
            ObservableCollection<DailyNoteEntry> entries = await dailyNoteService.GetDailyNoteEntryCollectionAsync().ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            UserAndUids = roles;
            DailyNoteEntries = entries;
            return true;
        }
        catch (Core.ExceptionService.UserdataCorruptedException ex)
        {
            infoBarService.Error(ex);
            return false;
        }
    }

    [Command("TrackRoleCommand")]
    private async Task TrackRoleAsync(UserAndUid? userAndUid)
    {
        if (userAndUid is not null)
        {
            ContentDialog dialog = await contentDialogFactory
                .CreateForIndeterminateProgressAsync(SH.ViewModelDailyNoteRequestProgressTitle)
                .ConfigureAwait(false);

            using (await dialog.BlockAsync(taskContext).ConfigureAwait(false))
            {
                await dailyNoteService.AddDailyNoteAsync(userAndUid).ConfigureAwait(false);
            }
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
            using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
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
        dialog.Text = options.WebhookUrl;
        (bool isOk, string url) = await dialog.GetInputUrlAsync().ConfigureAwait(false);

        if (isOk)
        {
            await taskContext.SwitchToMainThreadAsync();
            options.WebhookUrl = url;
            infoBarService.Information(SH.ViewModelDailyNoteConfigWebhookUrlComplete);
        }
    }
}