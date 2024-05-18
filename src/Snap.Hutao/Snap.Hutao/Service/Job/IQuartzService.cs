// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Quartz;

namespace Snap.Hutao.Service.Job;

internal interface IQuartzService
{
    ValueTask StartAsync(CancellationToken token = default);

    ValueTask StopJobAsync(string group, string triggerName, CancellationToken token = default);

    ValueTask UpdateJobAsync(string group, string triggerName, Func<TriggerBuilder, TriggerBuilder> configure, CancellationToken token = default);
}