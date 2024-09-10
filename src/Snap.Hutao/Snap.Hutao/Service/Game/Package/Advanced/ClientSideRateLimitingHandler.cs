// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Net.Http;
using System.Threading.RateLimiting;

namespace Snap.Hutao.Service.Game.Package.Advanced;

internal sealed partial class ClientSideRateLimitingHandler : DelegatingHandler, IAsyncDisposable
{
    private readonly RateLimiter limiter;

    public ClientSideRateLimitingHandler()
    {
        limiter = new TokenBucketRateLimiter(new()
        {
            ReplenishmentPeriod = TimeSpan.FromMilliseconds(200),
            TokensPerPeriod = 1,
            AutoReplenishment = true,
            TokenLimit = 50,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        });
    }

    public async ValueTask DisposeAsync()
    {
        await limiter.DisposeAsync().ConfigureAwait(false);

        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        do
        {
            using (RateLimitLease lease = await limiter.AcquireAsync(1, token).ConfigureAwait(false))
            {
                if (lease.IsAcquired)
                {
                    return await base.SendAsync(request, token).ConfigureAwait(false);
                }

                if (lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
                {
                    await Task.Delay(retryAfter, token).ConfigureAwait(false);
                }
            }
        }
        while (true);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            limiter.Dispose();
        }
    }
}