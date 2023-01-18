// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
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
[Injection(InjectAs.Scoped)]
internal class DailyNoteViewModel : Abstraction.ViewModel
{
    private readonly IUserService userService;
    private readonly IDailyNoteService dailyNoteService;
    private readonly AppDbContext appDbContext;

    private readonly List<NamedValue<int>> refreshTimes = new()
    {
        new("4 分钟 | 0.5 树脂", 240),
        new("8 分钟 | 1 树脂", 480),
        new("30 分钟 | 3.75 树脂", 1800),
        new("40 分钟 | 5 树脂", 2400),
        new("60 分钟 | 7.5 树脂", 3600),
    };

    private bool isReminderNotification;
    private NamedValue<int>? selectedRefreshTime;
    private ObservableCollection<UserAndUid>? userAndUids;

    private SettingEntry? refreshSecondsEntry;
    private SettingEntry? reminderNotifyEntry;
    private SettingEntry? silentModeEntry;
    private ObservableCollection<DailyNoteEntry>? dailyNoteEntries;
    private bool isSilentWhenPlayingGame;

    /// <summary>
    /// 构造一个新的实时便笺视图模型
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="dailyNoteService">实时便笺服务</param>
    /// <param name="appDbContext">数据库上下文</param>
    public DailyNoteViewModel(
        IUserService userService,
        IDailyNoteService dailyNoteService,
        AppDbContext appDbContext)
    {
        this.userService = userService;
        this.dailyNoteService = dailyNoteService;
        this.appDbContext = appDbContext;

        OpenUICommand = new AsyncRelayCommand(OpenUIAsync);
        TrackRoleCommand = new AsyncRelayCommand<UserAndUid>(TrackRoleAsync);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        RemoveDailyNoteCommand = new RelayCommand<DailyNoteEntry>(RemoveDailyNote);
        ModifyNotificationCommand = new AsyncRelayCommand<DailyNoteEntry>(ModifyDailyNoteNotificationAsync);
        DailyNoteVerificationCommand = new AsyncRelayCommand(VerifyDailyNoteVerificationAsync);
    }

    /// <summary>
    /// 刷新时间
    /// </summary>
    public List<NamedValue<int>> RefreshTimes { get => refreshTimes; }

    /// <summary>
    /// 选中的刷新时间
    /// </summary>
    public NamedValue<int>? SelectedRefreshTime
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
                        Ioc.Default.GetRequiredService<IInfoBarService>().Warning("注册计划任务失败，请使用管理员模式重试");
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
    public ObservableCollection<UserAndUid>? UserAndUids { get => userAndUids; set => userAndUids = value; }

    /// <summary>
    /// 实时便笺集合
    /// </summary>
    public ObservableCollection<DailyNoteEntry>? DailyNoteEntries { get => dailyNoteEntries; set => SetProperty(ref dailyNoteEntries, value); }

    /// <summary>
    /// 打开界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

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

    private async Task OpenUIAsync()
    {
        try
        {
            UserAndUids = await userService.GetRoleCollectionAsync().ConfigureAwait(false);
        }
        catch (Core.ExceptionService.UserdataCorruptedException ex)
        {
            Ioc.Default.GetRequiredService<IInfoBarService>().Error(ex);
            return;
        }

        try
        {
            ThrowIfViewDisposed();
            using (await DisposeLock.EnterAsync().ConfigureAwait(false))
            {
                ThrowIfViewDisposed();
                await ThreadHelper.SwitchToMainThreadAsync();

                refreshSecondsEntry = appDbContext.Settings.SingleOrAdd(SettingEntry.DailyNoteRefreshSeconds, "480");
                selectedRefreshTime = refreshTimes.Single(t => t.Value == refreshSecondsEntry.GetInt32());
                OnPropertyChanged(nameof(SelectedRefreshTime));
                ScheduleTaskHelper.RegisterForDailyNoteRefresh(480);

                reminderNotifyEntry = appDbContext.Settings.SingleOrAdd(SettingEntry.DailyNoteReminderNotify, SettingEntryHelper.FalseString);
                isReminderNotification = reminderNotifyEntry.GetBoolean();
                OnPropertyChanged(nameof(IsReminderNotification));

                silentModeEntry = appDbContext.Settings.SingleOrAdd(SettingEntry.DailyNoteSilentWhenPlayingGame, SettingEntryHelper.FalseString);
                isSilentWhenPlayingGame = silentModeEntry.GetBoolean();
                OnPropertyChanged(nameof(IsSilentWhenPlayingGame));

                await ThreadHelper.SwitchToBackgroundAsync();
            }

            ObservableCollection<DailyNoteEntry> temp = await dailyNoteService.GetDailyNoteEntriesAsync().ConfigureAwait(false);
            await ThreadHelper.SwitchToMainThreadAsync();
            DailyNoteEntries = temp;
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

    private void RemoveDailyNote(DailyNoteEntry? entry)
    {
        if (entry != null)
        {
            dailyNoteService.RemoveDailyNote(entry);
        }
    }

    private async Task ModifyDailyNoteNotificationAsync(DailyNoteEntry? entry)
    {
        if (entry != null)
        {
            // ContentDialog must be created by main thread.
            await ThreadHelper.SwitchToMainThreadAsync();
            await new DailyNoteNotificationDialog(entry).ShowAsync();
            appDbContext.DailyNotes.UpdateAndSave(entry);
        }
    }

    private async Task VerifyDailyNoteVerificationAsync()
    {
        if (UserAndUid.TryFromUser(userService.Current, out UserAndUid? userAndUid))
        {
            // ContentDialog must be created by main thread.
            await ThreadHelper.SwitchToMainThreadAsync();
            await new DailyNoteVerificationDialog(userAndUid).ShowAsync();
        }
        else
        {
            Ioc.Default.GetRequiredService<IInfoBarService>().Warning("请先选中账号与角色");
        }
    }
}
