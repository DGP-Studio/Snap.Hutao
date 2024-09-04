// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Quartz;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Job;
using System.Globalization;

namespace Snap.Hutao.Service.DailyNote;

[ConstructorGenerated(CallBaseConstructor = true)]
[Injection(InjectAs.Singleton)]
internal sealed partial class DailyNoteOptions : DbStoreOptions
{
    private const int OneMinute = 60;

    private readonly List<NameValue<int>> refreshTimes =
    [
        new(SH.ViewModelDailyNoteRefreshTime4, OneMinute * 4),
        new(SH.ViewModelDailyNoteRefreshTime8, OneMinute * 8),
        new(SH.ViewModelDailyNoteRefreshTime30, OneMinute * 30),
        new(SH.ViewModelDailyNoteRefreshTime40, OneMinute * 40),
        new(SH.ViewModelDailyNoteRefreshTime60, OneMinute * 60),
    ];

    private readonly IQuartzService quartzService;

    private bool? isAutoRefreshEnabled;
    private NameValue<int>? selectedRefreshTime;
    private bool? isReminderNotification;
    private bool? isSilentWhenPlayingGame;
    private string? webhookUrl;

    public List<NameValue<int>> RefreshTimes { get => refreshTimes; }

    public bool IsAutoRefreshEnabled
    {
        get => GetOption(ref isAutoRefreshEnabled, SettingEntry.DailyNoteIsAutoRefreshEnabled, false);
        set
        {
            if (SetOption(ref isAutoRefreshEnabled, SettingEntry.DailyNoteIsAutoRefreshEnabled, value))
            {
                if (value)
                {
                    if (SelectedRefreshTime is not null)
                    {
                        quartzService.UpdateJobAsync(JobIdentity.DailyNoteGroupName, JobIdentity.DailyNoteRefreshTriggerName, builder =>
                        {
                            return builder.WithSimpleSchedule(sb => sb.WithIntervalInSeconds(SelectedRefreshTime.Value).RepeatForever());
                        }).GetAwaiter().GetResult();
                    }
                }
                else
                {
                    quartzService.StopJobAsync(JobIdentity.DailyNoteGroupName, JobIdentity.DailyNoteRefreshTriggerName).GetAwaiter().GetResult();
                }
            }
        }
    }

    public NameValue<int>? SelectedRefreshTime
    {
        get => GetOption(ref selectedRefreshTime, SettingEntry.DailyNoteRefreshSeconds, time => RefreshTimes.Single(t => t.Value == int.Parse(time, CultureInfo.InvariantCulture)), RefreshTimes[1]);
        set
        {
            if (value is not null)
            {
                SetOption(ref selectedRefreshTime, SettingEntry.DailyNoteRefreshSeconds, value, v => $"{v.Value}");
                quartzService.UpdateJobAsync(JobIdentity.DailyNoteGroupName, JobIdentity.DailyNoteRefreshTriggerName, builder =>
                {
                    return builder.WithSimpleSchedule(sb => sb.WithIntervalInSeconds(value.Value).RepeatForever());
                }).GetAwaiter().GetResult();
            }
        }
    }

    public bool IsReminderNotification
    {
        get => GetOption(ref isReminderNotification, SettingEntry.DailyNoteReminderNotify);
        set => SetOption(ref isReminderNotification, SettingEntry.DailyNoteReminderNotify, value);
    }

    public bool IsSilentWhenPlayingGame
    {
        get => GetOption(ref isSilentWhenPlayingGame, SettingEntry.DailyNoteSilentWhenPlayingGame);
        set => SetOption(ref isSilentWhenPlayingGame, SettingEntry.DailyNoteSilentWhenPlayingGame, value);
    }

    public string? WebhookUrl
    {
        get => GetOption(ref webhookUrl, SettingEntry.DailyNoteWebhookUrl);
        set => SetOption(ref webhookUrl, SettingEntry.DailyNoteWebhookUrl, value);
    }
}