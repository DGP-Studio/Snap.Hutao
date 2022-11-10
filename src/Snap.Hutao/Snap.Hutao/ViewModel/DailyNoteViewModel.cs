// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Control;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.DailyNote;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using System.Collections.ObjectModel;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 实时便笺视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class DailyNoteViewModel : ObservableObject, ISupportCancellation
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
    private ObservableCollection<UserAndRole>? userAndRoles;

    private SettingEntry? refreshSecondsEntry;
    private SettingEntry? reminderNotifyEntry;
    private ObservableCollection<DailyNoteEntry>? dailyNoteEntries;

    /// <summary>
    /// 构造一个新的实时便笺视图模型
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="dailyNoteService">实时便笺服务</param>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    public DailyNoteViewModel(
        IUserService userService,
        IDailyNoteService dailyNoteService,
        AppDbContext appDbContext,
        IAsyncRelayCommandFactory asyncRelayCommandFactory)
    {
        this.userService = userService;
        this.dailyNoteService = dailyNoteService;
        this.appDbContext = appDbContext;

        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
        TrackRoleCommand = asyncRelayCommandFactory.Create<UserAndRole>(TrackRoleAsync);
        RefreshCommand = asyncRelayCommandFactory.Create(RefreshAsync);
        RemoveDailyNoteCommand = new RelayCommand<DailyNoteEntry>(RemoveDailyNote);
        ModifyNotificationCommand = asyncRelayCommandFactory.Create<DailyNoteEntry>(ModifyDailyNoteNotificationAsync);
    }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

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
                    refreshSecondsEntry!.SetInt32(value.Value);
                    appDbContext.Settings.UpdateAndSave(refreshSecondsEntry!);
                    TaskSchedulerHelper.RegisterForDailyNoteRefresh(value.Value);
                }
            }
        }
    }

    /// <summary>
    /// 提醒式通知
    /// </summary>
    public bool IsReminderNotification { get => isReminderNotification; set => SetProperty(ref isReminderNotification, value); }

    /// <summary>
    /// 用户与角色集合
    /// </summary>
    public ObservableCollection<UserAndRole>? UserAndRoles { get => userAndRoles; set => userAndRoles = value; }

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

    private async Task OpenUIAsync()
    {
        UserAndRoles = await userService.GetRoleCollectionAsync().ConfigureAwait(true);

        refreshSecondsEntry = appDbContext.Settings.SingleOrAdd(SettingEntry.DailyNoteRefreshSeconds, "480");
        selectedRefreshTime = refreshTimes.Single(t => t.Value == refreshSecondsEntry.GetInt32());
        TaskSchedulerHelper.RegisterForDailyNoteRefresh(480);
        OnPropertyChanged(nameof(SelectedRefreshTime));

        reminderNotifyEntry = appDbContext.Settings.SingleOrAdd(SettingEntry.DailyNoteReminderNotify, false.ToString());
        isReminderNotification = reminderNotifyEntry.GetBoolean();
        OnPropertyChanged(nameof(IsReminderNotification));

        DailyNoteEntries = await dailyNoteService.GetDailyNoteEntriesAsync().ConfigureAwait(true);
    }

    private async Task TrackRoleAsync(UserAndRole? role)
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
            MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
            await new DailyNoteNotificationDialog(mainWindow, entry).ShowAsync();
            appDbContext.DailyNotes.UpdateAndSave(entry);
        }
    }
}