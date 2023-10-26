// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
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
[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Singleton)]
internal sealed partial class DailyNoteOptions : DbStoreOptions
{
    private const int OneMinute = 60;

    private readonly RuntimeOptions runtimeOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly IScheduleTaskInterop scheduleTaskInterop;

    private NameValue<int>? selectedRefreshTime;
    private bool? isReminderNotification;
    private bool? isSilentWhenPlayingGame;
    private string? webhookUrl;

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
            if (runtimeOptions.IsElevated)
            {
                // leave below untouched if we are running in elevated privilege
                return;
            }

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
        get
        {
            if (runtimeOptions.IsElevated)
            {
                // leave untouched when we are running in elevated privilege
                return null;
            }

            return GetOption(ref selectedRefreshTime, SettingEntry.DailyNoteRefreshSeconds, time => RefreshTimes.Single(t => t.Value == int.Parse(time, CultureInfo.InvariantCulture)), RefreshTimes[1]);
        }

        set
        {
            if (runtimeOptions.IsElevated)
            {
                // leave untouched when we are running in elevated privilege
                return;
            }

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

    public string? WebhookUrl
    {
        get => GetOption(ref webhookUrl, SettingEntry.DailyNoteSilentWhenPlayingGame);
        set => SetOption(ref webhookUrl, SettingEntry.DailyNoteSilentWhenPlayingGame, value);
    }
}