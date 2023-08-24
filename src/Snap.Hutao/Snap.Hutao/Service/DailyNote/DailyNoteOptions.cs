// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Shell;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Notification;
using System.Globalization;

namespace Snap.Hutao.Service.DailyNote;

/// <summary>
/// 实时便笺选项
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed class DailyNoteOptions : DbStoreOptions
{
    private const int OneMinute = 60;

    private readonly IServiceProvider serviceProvider;
    private readonly IScheduleTaskInterop scheduleTaskInterop;

    private NameValue<int>? selectedRefreshTime;
    private bool? isReminderNotification;
    private bool? isSilentWhenPlayingGame;

    /// <summary>
    /// 构造一个新的实时便笺选项
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public DailyNoteOptions(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        scheduleTaskInterop = serviceProvider.GetRequiredService<IScheduleTaskInterop>();
        this.serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 刷新时间
    /// </summary>
    public List<NameValue<int>> RefreshTimes { get; } = new()
    {
        new(SH.ViewModelDailyNoteRefreshTime4, OneMinute * 4),
        new(SH.ViewModelDailyNoteRefreshTime8, OneMinute * 8),
        new(SH.ViewModelDailyNoteRefreshTime30, OneMinute * 30),
        new(SH.ViewModelDailyNoteRefreshTime40, OneMinute * 40),
        new(SH.ViewModelDailyNoteRefreshTime60, OneMinute * 60),
    };

    public bool IsAutoRefreshEnabled
    {
        get => scheduleTaskInterop.IsDailyNoteRefreshEnabled();
        set
        {
            if (value)
            {
                if (SelectedRefreshTime is not null)
                {
                    scheduleTaskInterop.RegisterForDailyNoteRefresh(SelectedRefreshTime.Value);
                }
            }
            else
            {
                scheduleTaskInterop.UnregisterForDailyNoteRefresh();
            }

            OnPropertyChanged();
        }
    }

    /// <summary>
    /// 选中的刷新时间
    /// </summary>
    public NameValue<int>? SelectedRefreshTime
    {
        get => GetOption(ref selectedRefreshTime, SettingEntry.DailyNoteRefreshSeconds, time => RefreshTimes.Single(t => t.Value == int.Parse(time, CultureInfo.InvariantCulture)), RefreshTimes[1]);
        set
        {
            if (value is not null)
            {
                if (scheduleTaskInterop.RegisterForDailyNoteRefresh(value.Value))
                {
                    SetOption(ref selectedRefreshTime, SettingEntry.DailyNoteRefreshSeconds, value, value => $"{value.Value}");
                }
                else
                {
                    serviceProvider.GetRequiredService<IInfoBarService>().Warning(SH.ViewModelDailyNoteRegisterTaskFail);
                }
            }
        }
    }

    /// <summary>
    /// 提醒式通知
    /// </summary>
    public bool IsReminderNotification
    {
        get => GetOption(ref isReminderNotification, SettingEntry.DailyNoteReminderNotify);
        set => SetOption(ref isReminderNotification, SettingEntry.DailyNoteReminderNotify, value);
    }

    /// <summary>
    /// 是否开启免打扰模式
    /// </summary>
    public bool IsSilentWhenPlayingGame
    {
        get => GetOption(ref isSilentWhenPlayingGame, SettingEntry.DailyNoteSilentWhenPlayingGame);
        set => SetOption(ref isSilentWhenPlayingGame, SettingEntry.DailyNoteSilentWhenPlayingGame, value);
    }
}