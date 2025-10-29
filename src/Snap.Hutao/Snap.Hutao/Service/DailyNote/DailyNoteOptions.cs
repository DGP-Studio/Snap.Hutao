// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Quartz;
using Snap.Hutao.Core.Property;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Job;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.DailyNote;

[GeneratedConstructor(CallBaseConstructor = true)]
[Service(ServiceLifetime.Singleton)]
internal sealed partial class DailyNoteOptions : DbStoreOptions
{
    private const int OneMinute = 60;

    private readonly IQuartzService quartzService;

    public ImmutableArray<NameValue<int>> RefreshTimes { get; } =
    [
        new(SH.ViewModelDailyNoteRefreshTime4, OneMinute * 4),
        new(SH.ViewModelDailyNoteRefreshTime8, OneMinute * 8),
        new(SH.ViewModelDailyNoteRefreshTime30, OneMinute * 30),
        new(SH.ViewModelDailyNoteRefreshTime40, OneMinute * 40),
        new(SH.ViewModelDailyNoteRefreshTime60, OneMinute * 60),
    ];

    [field: MaybeNull]
    public IObservableProperty<bool> IsAutoRefreshEnabled { get => field ??= CreateProperty(SettingEntry.DailyNoteIsAutoRefreshEnabled, false).WithValueChangedCallback(OnIsAutoRefreshEnabledChanged, this); }

    [field: MaybeNull]
    public IObservableProperty<NameValue<int>?> SelectedRefreshTime { get => field ??= CreateProperty(SettingEntry.DailyNoteRefreshSeconds, OneMinute * 30).WithValueChangedCallback(OnSelectedRefreshTimeChanged, this).AsNameValue(RefreshTimes); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsReminderNotification { get => field ??= CreateProperty(SettingEntry.DailyNoteReminderNotify, false); }

    [field: MaybeNull]
    public IObservableProperty<bool> IsSilentWhenPlayingGame { get => field ??= CreateProperty(SettingEntry.DailyNoteSilentWhenPlayingGame, false); }

    [field: MaybeNull]
    public IObservableProperty<string?> WebhookUrl { get => field ??= CreateProperty(SettingEntry.DailyNoteWebhookUrl); }

    private static void OnIsAutoRefreshEnabledChanged(bool value, DailyNoteOptions options)
    {
        if (value)
        {
            if (options.SelectedRefreshTime.Value is not null)
            {
                int refreshTime = options.SelectedRefreshTime.Value.Value;
                options.quartzService.UpdateJobAsync(JobIdentity.DailyNoteGroupName, JobIdentity.DailyNoteRefreshTriggerName, builder =>
                {
                    return builder.WithSimpleSchedule(sb => sb.WithIntervalInSeconds(refreshTime).RepeatForever());
                }).GetAwaiter().GetResult();
            }
        }
        else
        {
            options.quartzService.StopJobAsync(JobIdentity.DailyNoteGroupName, JobIdentity.DailyNoteRefreshTriggerName).GetAwaiter().GetResult();
        }
    }

    private static void OnSelectedRefreshTimeChanged(int value, DailyNoteOptions options)
    {
        options.quartzService.UpdateJobAsync(JobIdentity.DailyNoteGroupName, JobIdentity.DailyNoteRefreshTriggerName, builder =>
        {
            return builder.WithSimpleSchedule(sb => sb.WithIntervalInSeconds(value).RepeatForever());
        }).GetAwaiter().GetResult();
    }
}