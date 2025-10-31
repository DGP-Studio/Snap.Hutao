// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Quartz;
using Snap.Hutao.Service.DailyNote;

namespace Snap.Hutao.Service.Job;

[Service(ServiceLifetime.Transient, typeof(IJobScheduler))]
internal sealed partial class DailyNoteRefreshJobScheduler : IJobScheduler
{
    private readonly DailyNoteOptions dailyNoteOptions;

    [GeneratedConstructor]
    public partial DailyNoteRefreshJobScheduler(IServiceProvider serviceProvider);

    public async ValueTask ScheduleAsync(IScheduler scheduler)
    {
        if (!TryGetRefreshInterval(out int interval))
        {
            return;
        }

        IJobDetail dailyNoteJob = JobBuilder.Create<DailyNoteRefreshJob>()
            .WithIdentity(JobIdentity.DailyNoteRefreshJobName, JobIdentity.DailyNoteGroupName)
            .Build();

        ITrigger dailyNoteTrigger = TriggerBuilder.Create()
            .WithIdentity(JobIdentity.DailyNoteRefreshTriggerName, JobIdentity.DailyNoteGroupName)
            .WithSimpleSchedule(builder => builder.WithIntervalInSeconds(interval).RepeatForever())
            .StartAt(DateTimeOffset.Now.AddSeconds(interval))
            .Build();

        await scheduler.ScheduleJob(dailyNoteJob, dailyNoteTrigger).ConfigureAwait(false);
    }

    private bool TryGetRefreshInterval(out int interval)
    {
        if (dailyNoteOptions is { IsAutoRefreshEnabled.Value: true, SelectedRefreshTime.Value: { } refreshTime })
        {
            interval = refreshTime.Value;
            return true;
        }

        interval = 0;
        return false;
    }
}