// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service;
using System.Threading.RateLimiting;

namespace Snap.Hutao.Core.IO;

[Injection(InjectAs.Singleton)]
[SuppressMessage("", "CA1001")]
internal sealed partial class StreamCopySpeedLimiter
{
    private TokenBucketRateLimiter rateLimiter = default!;

    public StreamCopySpeedLimiter(IServiceProvider serviceProvider)
    {
        AppOptions appOptions = serviceProvider.GetRequiredService<AppOptions>();

        SetRateLimit(appOptions.DownloadBytesPerSecondLimit);
    }

    public TokenBucketRateLimiter RateLimiter { get => rateLimiter; }

    public void SetRateLimit(int bytesPerSecond)
    {
        bytesPerSecond = bytesPerSecond * 1024;
        if (bytesPerSecond <= 0)
        {
            bytesPerSecond = int.MaxValue;
        }

        TokenBucketRateLimiterOptions options = new()
        {
            TokenLimit = bytesPerSecond,
            ReplenishmentPeriod = TimeSpan.FromMilliseconds(100),
            TokensPerPeriod = bytesPerSecond / 10,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            AutoReplenishment = true,
        };

        rateLimiter = new TokenBucketRateLimiter(options);
    }
}