// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Threading.RateLimiting;

namespace Snap.Hutao.Core.Threading.RateLimiting;

internal static class ProgressReportRateLimiter
{
    public static TokenBucketRateLimiter Create(int replenishmentMilliSeconds)
    {
        return new(new()
        {
            ReplenishmentPeriod = TimeSpan.FromMilliseconds(replenishmentMilliSeconds),
            TokensPerPeriod = 1,
            AutoReplenishment = true,
            TokenLimit = 1,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        });
    }
}