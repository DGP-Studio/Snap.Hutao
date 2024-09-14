// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ComponentModel;
using Snap.Hutao.Service;
using System.Threading.RateLimiting;

namespace Snap.Hutao.Core.Threading.RateLimiting;

internal static class StreamCopyRateLimiter
{
    private const double ReplenishmentCountPerSecond = 20;

    public static NotifyPropertyChangedBox<AppOptions, TokenBucketRateLimiter?> GetOrCreate(IServiceProvider serviceProvider)
    {
        AppOptions appOptions = serviceProvider.GetRequiredService<AppOptions>();
        return new(CreateCore(appOptions), appOptions, nameof(AppOptions.DownloadSpeedLimitPerSecondInKiloByte), CreateCore);
    }

    private static TokenBucketRateLimiter? CreateCore(AppOptions appOptions)
    {
        int bytesPerSecond = appOptions.DownloadSpeedLimitPerSecondInKiloByte * 1024;

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

        return new TokenBucketRateLimiter(options);
    }
}