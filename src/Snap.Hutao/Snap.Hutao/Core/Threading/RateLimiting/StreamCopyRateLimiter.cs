// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service;
using System.Threading.RateLimiting;

namespace Snap.Hutao.Core.Threading.RateLimiting;

internal static class StreamCopyRateLimiter
{
    private const double ReplenishmentCountPerSecond = 20;

    public static TokenBucketRateLimiter? Create(IServiceProvider serviceProvider)
    {
        return PrivateCreate(serviceProvider.GetRequiredService<AppOptions>());
    }

    private static TokenBucketRateLimiter? PrivateCreate(AppOptions appOptions)
    {
        int bytesPerSecond = appOptions.DownloadSpeedLimitPerSecondInKiloByte.Value * 1024;

        if (bytesPerSecond <= 0)
        {
            return default;
        }

        TokenBucketRateLimiterOptions options = new()
        {
            TokenLimit = bytesPerSecond,
            ReplenishmentPeriod = TimeSpan.FromMilliseconds(1000 / ReplenishmentCountPerSecond),
            TokensPerPeriod = (int)(bytesPerSecond / ReplenishmentCountPerSecond),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            AutoReplenishment = true,
        };

        return new(options);
    }
}