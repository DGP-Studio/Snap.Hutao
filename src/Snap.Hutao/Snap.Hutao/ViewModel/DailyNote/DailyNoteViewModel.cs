// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.DailyNote;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.ViewModel.User;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel.DailyNote;

/// <summary>
/// 实时便笺视图模型
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped)]
internal sealed class DailyNoteViewModel : Abstraction.ViewModel
{
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly IDailyNoteService dailyNoteService;
    private readonly AppDbContext appDbContext;

    private ObservableCollection<UserAndUid>? userAndUids;
    private ObservableCollection<DailyNoteEntry>? dailyNoteEntries;

    /// <summary>
    /// 构造一个新的实时便笺视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public DailyNoteViewModel(IServiceProvider serviceProvider)
    {
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
        userService = serviceProvider.GetRequiredService<IUserService>();
        dailyNoteService = serviceProvider.GetRequiredService<IDailyNoteService>();
        appDbContext = serviceProvider.GetRequiredService<AppDbContext>();
        Options = serviceProvider.GetRequiredService<DailyNoteOptions>();
        this.serviceProvider = serviceProvider;

        TrackRoleCommand = new AsyncRelayCommand<UserAndUid>(TrackRoleAsync);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        RemoveDailyNoteCommand = new AsyncRelayCommand<DailyNoteEntry>(RemoveDailyNoteAsync);
        ModifyNotificationCommand = new AsyncRelayCommand<DailyNoteEntry>(ModifyDailyNoteNotificationAsync);
        DailyNoteVerificationCommand = new AsyncRelayCommand(VerifyDailyNoteVerificationAsync);
    }

    /// <summary>
    /// 选项
    /// </summary>
    public DailyNoteOptions Options { get; }

    /// <summary>
    /// 用户与角色集合
    /// </summary>
    public ObservableCollection<UserAndUid>? UserAndUids { get => userAndUids; set => SetProperty(ref userAndUids, value); }

    /// <summary>
    /// 实时便笺集合
    /// </summary>
    public ObservableCollection<DailyNoteEntry>? DailyNoteEntries { get => dailyNoteEntries; set => SetProperty(ref dailyNoteEntries, value); }

    /// <summary>
    /// 跟踪角色命令
    /// </summary>
    public ICommand TrackRoleCommand { get; }

    /// <summary>
    /// 刷新命令
    /// </summary>
    public ICommand RefreshCommand { get; }

    /// <summary>
    /// 移除实时便笺命令
    /// </summary>
    public ICommand RemoveDailyNoteCommand { get; }

    /// <summary>
    /// 修改实时便笺通知命令
    /// </summary>
    public ICommand ModifyNotificationCommand { get; }

    /// <summary>
    /// 验证实时便笺命令
    /// </summary>
    public ICommand DailyNoteVerificationCommand { get; }

    /// <inheritdoc/>
    protected override async Task OpenUIAsync()
    {
        try
        {
            await taskContext.SwitchToBackgroundAsync();
            ObservableCollection<UserAndUid> roles = await userService.GetRoleCollectionAsync().ConfigureAwait(false);
            ObservableCollection<DailyNoteEntry> entries = await dailyNoteService.GetDailyNoteEntriesAsync().ConfigureAwait(false);

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

    private async Task TrackRoleAsync(UserAndUid? role)
    {
        if (role != null)
        {
            await dailyNoteService.AddDailyNoteAsync(role).ConfigureAwait(false);
        }
    }

    private async Task RefreshAsync()
    {
        await dailyNoteService.RefreshDailyNotesAsync().ConfigureAwait(false);
    }

    private async Task RemoveDailyNoteAsync(DailyNoteEntry? entry)
    {
        if (entry != null)
        {
            await dailyNoteService.RemoveDailyNoteAsync(entry).ConfigureAwait(false);
        }
    }

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