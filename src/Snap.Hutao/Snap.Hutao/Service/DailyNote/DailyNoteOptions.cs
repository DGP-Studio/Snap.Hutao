// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Quartz;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Abstraction.Property;
using Snap.Hutao.Service.Job;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.DailyNote;

[ConstructorGenerated(CallBaseConstructor = true)]
[Service(ServiceLifetime.Singleton)]
internal sealed partial class DailyNoteOptions : DbStoreOptions
{
    private const int OneMinute = 60;

    private readonly IQuartzService quartzService;

    private bool? isAutoRefreshEnabled;

    public ImmutableArray<NameValue<int>> RefreshTimes { get; } =
    [
        new(SH.ViewModelDailyNoteRefreshTime4, OneMinute * 4),
        new(SH.ViewModelDailyNoteRefreshTime8, OneMinute * 8),
        new(SH.ViewModelDailyNoteRefreshTime30, OneMinute * 30),
        new(SH.ViewModelDailyNoteRefreshTime40, OneMinute * 40),
        new(SH.ViewModelDailyNoteRefreshTime60, OneMinute * 60),
    ];

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
                        int refreshTime = SelectedRefreshTime.Value;
                        quartzService.UpdateJobAsync(JobIdentity.DailyNoteGroupName, JobIdentity.DailyNoteRefreshTriggerName, builder =>
                        {
                            return builder.WithSimpleSchedule(sb => sb.WithIntervalInSeconds(refreshTime).RepeatForever());
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
        get => GetOption(ref field, SettingEntry.DailyNoteRefreshSeconds, RefreshTimes, static v => $"{v}", RefreshTimes[1]);
        set
        {
            if (value is not null)
            {
                SetOption(ref field, SettingEntry.DailyNoteRefreshSeconds, value, static v => $"{v.Value}");
                quartzService.UpdateJobAsync(JobIdentity.DailyNoteGroupName, JobIdentity.DailyNoteRefreshTriggerName, builder =>
                {
                    return builder.WithSimpleSchedule(sb => sb.WithIntervalInSeconds(value.Value).RepeatForever());
                }).GetAwaiter().GetResult();
            }
        }
    }

    [field: MaybeNull]
    public DbProperty<bool> IsReminderNotification { get => field ??= CreateProperty(SettingEntry.DailyNoteReminderNotify, false); }

    [field: MaybeNull]
    public DbProperty<bool> IsSilentWhenPlayingGame { get => field ??= CreateProperty(SettingEntry.DailyNoteSilentWhenPlayingGame, false); }

    [field: MaybeNull]
    public DbProperty<string?> WebhookUrl { get => field ??= CreateProperty(SettingEntry.DailyNoteWebhookUrl); }
}