// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Quartz;

namespace Snap.Hutao.Service.Job;

[SuppressMessage("", "SH003")]
internal interface IQuartzService
{
    Task StartAsync(CancellationToken token = default);

    Task StopJobAsync(string group, string triggerName, CancellationToken token = default);

    Task UpdateJobAsync(string group, string triggerName, Func<TriggerBuilder, TriggerBuilder> configure, CancellationToken token = default);
}