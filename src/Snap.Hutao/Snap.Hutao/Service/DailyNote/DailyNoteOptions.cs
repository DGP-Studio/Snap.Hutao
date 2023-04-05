// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.DailyNote;

/// <summary>
/// 实时便笺选项
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed class DailyNoteOptions : DbStoreOptions
{
    private readonly IServiceProvider serviceProvider;
    private readonly List<NameValue<int>> refreshTimes = new()
    {
        new(SH.ViewModelDailyNoteRefreshTime4, 240),
        new(SH.ViewModelDailyNoteRefreshTime8, 480),
        new(SH.ViewModelDailyNoteRefreshTime30, 1800),
        new(SH.ViewModelDailyNoteRefreshTime40, 2400),
        new(SH.ViewModelDailyNoteRefreshTime60, 3600),
    };

    private NameValue<int>? selectedRefreshTime;
    private bool? isReminderNotification;
    private bool? isSilentWhenPlayingGame;

    /// <summary>
    /// 构造一个新的实时便笺选项
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public DailyNoteOptions(IServiceProvider serviceProvider)
        : base(serviceProvider.GetRequiredService<IServiceScopeFactory>())
    {
        this.serviceProvider = serviceProvider;
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
        get => GetOption(ref selectedRefreshTime, SettingEntry.DailyNoteRefreshSeconds, time => RefreshTimes.Single(t => t.Value == int.Parse(time)), RefreshTimes[1]);
        set
        {
            if (value != null)
            {
                if (ScheduleTaskHelper.RegisterForDailyNoteRefresh(value.Value))
                {
                    SetOption(ref selectedRefreshTime, SettingEntry.DailyNoteRefreshSeconds, value, value => value.Value.ToString());
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