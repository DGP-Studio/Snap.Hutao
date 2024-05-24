// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Quartz;

namespace Snap.Hutao.Service.Job;

[Injection(InjectAs.Singleton, typeof(IQuartzService))]
[ConstructorGenerated]
internal sealed partial class QuartzService : IQuartzService, IDisposable
{
    private readonly TaskCompletionSource startupCompleted = new();

    private readonly ISchedulerFactory schedulerFactory;
    private readonly IServiceProvider serviceProvider;

    private IScheduler? scheduler;

    public async ValueTask StartAsync(CancellationToken token = default)
    {
        scheduler = await schedulerFactory.GetScheduler(token).ConfigureAwait(false);
        await scheduler.Start(token).ConfigureAwait(false);

        foreach (IJobScheduler jobScheduler in serviceProvider.GetServices<IJobScheduler>())
        {
            await jobScheduler.ScheduleAsync(scheduler).ConfigureAwait(false);
        }

        startupCompleted.SetResult();
    }

    public async ValueTask UpdateJobAsync(string group, string triggerName, Func<TriggerBuilder, TriggerBuilder> configure, CancellationToken token = default)
    {
        if (scheduler is null)
        {
            return;
        }

        await startupCompleted.Task.ConfigureAwait(false);

        TriggerKey key = new(triggerName, group);
        if (await scheduler.GetTrigger(key, token).ConfigureAwait(false) is { } old)
        {
            ITrigger newTrigger = configure(old.GetTriggerBuilder()).Build();
            await scheduler.RescheduleJob(key, newTrigger, token).ConfigureAwait(false);
        }
    }

    public async ValueTask StopJobAsync(string group, string triggerName, CancellationToken token = default)
    {
        if (scheduler is null)
        {
            return;
        }

        await startupCompleted.Task.ConfigureAwait(false);

        await scheduler.UnscheduleJob(new(triggerName, group), token).ConfigureAwait(false);
    }

    public void Dispose()
    {
        DisposeAsync().GetAwaiter().GetResult();

        async Task DisposeAsync()
        {
            if (scheduler is null)
            {
                return;
            }

            try
            {
                // Wait until any ongoing startup logic has finished or the graceful shutdown period is over
                await startupCompleted.Task.ConfigureAwait(false);
            }
            finally
            {
                await scheduler.Shutdown(false).ConfigureAwait(false);
                scheduler = default;
            }
        }
    }
}