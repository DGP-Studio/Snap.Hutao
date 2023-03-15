// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.DailyNote;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 实时便笺视图模型
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped)]
internal sealed class DailyNoteViewModel : Abstraction.ViewModel
{
    private readonly IServiceProvider serviceProvider;
    private readonly IUserService userService;
    private readonly IDailyNoteService dailyNoteService;
    private readonly AppDbContext appDbContext;

    private readonly List<NameValue<int>> refreshTimes = new()
    {
        new(SH.ViewModelDailyNoteRefreshTime4, 240),
        new(SH.ViewModelDailyNoteRefreshTime8, 480),
        new(SH.ViewModelDailyNoteRefreshTime30, 1800),
        new(SH.ViewModelDailyNoteRefreshTime40, 2400),
        new(SH.ViewModelDailyNoteRefreshTime60, 3600),
    };

    private bool isReminderNotification;
    private NameValue<int>? selectedRefreshTime;
    private ObservableCollection<UserAndUid>? userAndUids;

    private SettingEntry? refreshSecondsEntry;
    private SettingEntry? reminderNotifyEntry;
    private SettingEntry? silentModeEntry;
    private ObservableCollection<DailyNoteEntry>? dailyNoteEntries;
    private bool isSilentWhenPlayingGame;

    /// <summary>
    /// 构造一个新的实时便笺视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public DailyNoteViewModel(IServiceProvider serviceProvider)
    {
        userService = serviceProvider.GetRequiredService<IUserService>();
        dailyNoteService = serviceProvider.GetRequiredService<IDailyNoteService>();
        appDbContext = serviceProvider.GetRequiredService<AppDbContext>();
        this.serviceProvider = serviceProvider;

        TrackRoleCommand = new AsyncRelayCommand<UserAndUid>(TrackRoleAsync);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        RemoveDailyNoteCommand = new AsyncRelayCommand<DailyNoteEntry>(RemoveDailyNoteAsync);
        ModifyNotificationCommand = new AsyncRelayCommand<DailyNoteEntry>(ModifyDailyNoteNotificationAsync);
        DailyNoteVerificationCommand = new AsyncRelayCommand(VerifyDailyNoteVerificationAsync);
    }

    /// <summary>
    /// 刷新时间
    /// </summary>
    public List<NameValue<int>> RefreshTimes { get => refreshTimes; }

    /// <summary>
    /// 选中的刷新时间
    /// </summary>
    public NameValue<int>? SelectedRefreshTime
    {
        get => selectedRefreshTime;
        set
        {
            if (SetProperty(ref selectedRefreshTime, value))
            {
                if (value != null)
                {
                    if (!ScheduleTaskHelper.RegisterForDailyNoteRefresh(value.Value))
                    {
                        serviceProvider.GetRequiredService<IInfoBarService>().Warning(SH.ViewModelDailyNoteRegisterTaskFail);
                    }
                    else
                    {
                        refreshSecondsEntry!.SetInt32(value.Value);
                        appDbContext.Settings.UpdateAndSave(refreshSecondsEntry!);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 提醒式通知
    /// </summary>
    public bool IsReminderNotification
    {
        get => isReminderNotification;
        set
        {
            if (SetProperty(ref isReminderNotification, value))
            {
                reminderNotifyEntry!.SetBoolean(value);
                appDbContext.Settings.UpdateAndSave(reminderNotifyEntry!);
            }
        }
    }

    /// <summary>
    /// 是否开启免打扰模式
    /// </summary>
    public bool IsSilentWhenPlayingGame
    {
        get => isSilentWhenPlayingGame;
        set
        {
            if (SetProperty(ref isSilentWhenPlayingGame, value))
            {
                silentModeEntry!.SetBoolean(value);
                appDbContext.Settings.UpdateAndSave(silentModeEntry!);
            }
        }
    }

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
            UserAndUids = await userService.GetRoleCollectionAsync().ConfigureAwait(true);
        }
        catch (Core.ExceptionService.UserdataCorruptedException ex)
        {
            serviceProvider.GetRequiredService<IInfoBarService>().Error(ex);
            return;
        }

        try
        {
            using (await EnterCriticalExecutionAsync().ConfigureAwait(false))
            {
                await ThreadHelper.SwitchToMainThreadAsync();

                refreshSecondsEntry = appDbContext.Settings.SingleOrAdd(SettingEntry.DailyNoteRefreshSeconds, "480");
                int refreshSeconds = refreshSecondsEntry.GetInt32();
                selectedRefreshTime = refreshTimes.Single(t => t.Value == refreshSeconds);
                OnPropertyChanged(nameof(SelectedRefreshTime));
                ScheduleTaskHelper.RegisterForDailyNoteRefresh(refreshSeconds);

                reminderNotifyEntry = appDbContext.Settings.SingleOrAdd(SettingEntry.DailyNoteReminderNotify, Core.StringLiterals.False);
                isReminderNotification = reminderNotifyEntry.GetBoolean();
                OnPropertyChanged(nameof(IsReminderNotification));

                silentModeEntry = appDbContext.Settings.SingleOrAdd(SettingEntry.DailyNoteSilentWhenPlayingGame, Core.StringLiterals.False);
                isSilentWhenPlayingGame = silentModeEntry.GetBoolean();
                OnPropertyChanged(nameof(IsSilentWhenPlayingGame));
            }

            await ThreadHelper.SwitchToBackgroundAsync();
            ObservableCollection<DailyNoteEntry> entries = await dailyNoteService.GetDailyNoteEntriesAsync().ConfigureAwait(false);
            await ThreadHelper.SwitchToMainThreadAsync();
            DailyNoteEntries = entries;
        }
        catch (OperationCanceledException)
        {
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
        await dailyNoteService.RefreshDailyNotesAsync(false).ConfigureAwait(false);
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
                await ThreadHelper.SwitchToMainThreadAsync();
                await new DailyNoteNotificationDialog(entry).ShowAsync();
                appDbContext.DailyNotes.UpdateAndSave(entry);
            }
        }
    }

    private async Task VerifyDailyNoteVerificationAsync()
    {
        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            // TODO: Add verify support for oversea user
            if (userAndUid.Uid.Region != "cn_gf01" || userAndUid.Uid.Region != "cn_qd01")
            {
                serviceProvider.GetRequiredService<IInfoBarService>().Warning("Unsupported for hoyoverse account");
            }
            else
            {
                // ContentDialog must be created by main thread.
                await ThreadHelper.SwitchToMainThreadAsync();
                await new DailyNoteVerificationDialog(userAndUid).ShowAsync();
            }
        }
        else
        {
            serviceProvider.GetRequiredService<IInfoBarService>().Warning(SH.MustSelectUserAndUid);
        }
    }
}