// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.DailyNote;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
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
    private readonly IDailyNoteService dailyNoteService;
    private readonly IServiceProvider serviceProvider;
    private readonly AppDbContext appDbContext;
    private readonly DailyNoteOptions options;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;

    private ObservableCollection<UserAndUid>? userAndUids;
    private ObservableCollection<DailyNoteEntry>? dailyNoteEntries;

    /// <summary>
    /// 选项
    /// </summary>
    public DailyNoteOptions Options { get => options; }

    /// <summary>
    /// 用户与角色集合
    /// </summary>
    public ObservableCollection<UserAndUid>? UserAndUids { get => userAndUids; set => SetProperty(ref userAndUids, value); }

    /// <summary>
    /// 实时便笺集合
    /// </summary>
    public ObservableCollection<DailyNoteEntry>? DailyNoteEntries { get => dailyNoteEntries; set => SetProperty(ref dailyNoteEntries, value); }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        try
        {
            await taskContext.SwitchToBackgroundAsync();
            ObservableCollection<UserAndUid> roles = await userService.GetRoleCollectionAsync().ConfigureAwait(false);
            ObservableCollection<DailyNoteEntry> entries = await dailyNoteService.GetDailyNoteEntryCollectionAsync().ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            UserAndUids = roles;
            DailyNoteEntries = entries;
        }
        catch (Core.ExceptionService.UserdataCorruptedException ex)
        {
            serviceProvider.GetRequiredService<IInfoBarService>().Error(ex);
            return;
        }
    }

    [Command("TrackRoleCommand")]
    private async Task TrackRoleAsync(UserAndUid? role)
    {
        if (role != null)
        {
            await dailyNoteService.AddDailyNoteAsync(role).ConfigureAwait(false);
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
        if (entry != null)
        {
            await dailyNoteService.RemoveDailyNoteAsync(entry).ConfigureAwait(false);
        }
    }

    [Command("ModifyNotificationCommand")]
    private async Task ModifyDailyNoteNotificationAsync(DailyNoteEntry? entry)
    {
        if (entry != null)
        {
            using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
            {
                // ContentDialog must be created by main thread.
                await taskContext.SwitchToMainThreadAsync();
                DailyNoteNotificationDialog dialog = serviceProvider.CreateInstance<DailyNoteNotificationDialog>(entry);
                await dialog.ShowAsync();
                appDbContext.DailyNotes.UpdateAndSave(entry);
            }
        }
    }

    [Command("DailyNoteVerificationCommand")]
    private async Task VerifyDailyNoteVerificationAsync()
    {
        IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();

        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            // TODO: Add verify support for oversea user
            if (userAndUid.User.IsOversea)
            {
                infoBarService.Warning(SH.ViewModelDailyNoteHoyolabVerificationUnsupported);
            }
            else
            {
                // ContentDialog must be created by main thread.
                await taskContext.SwitchToMainThreadAsync();
                DailyNoteVerificationDialog dialog = serviceProvider.CreateInstance<DailyNoteVerificationDialog>(userAndUid);
                await dialog.ShowAsync();
            }
        }
        else
        {
            infoBarService.Warning(SH.MustSelectUserAndUid);
        }
    }
}