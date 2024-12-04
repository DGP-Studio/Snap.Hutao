using System;
using System.Runtime.CompilerServices;
using System.Threading.RateLimiting;

namespace Snap.Hutao.Test.PlatformExtensions;

[TestClass]
public sealed class RateLimitingTest
{
    [TestMethod]
    public void TokenBucketRateLimiterPrivateGetQueueCanAsObject()
    {
        TokenBucketRateLimiter rateLimiter = new(new()
        {
            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
            TokensPerPeriod = 1,
            AutoReplenishment = true,
            TokenLimit = 1,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
        });

        using (rateLimiter)
        {
            lock(PrivateGetLock(rateLimiter))
            {
                Assert.IsNotNull(rateLimiter);
            }
        }
    }

    // private object Lock => _queue
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Lock")]
    private static extern object PrivateGetLock(TokenBucketRateLimiter rateLimiter);

    // private double _tokenCount;
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_tokenCount")]
    private static extern ref double PrivateGetTokenCount(TokenBucketRateLimiter rateLimiter);

    // private readonly TokenBucketRateLimiterOptions _options;
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_options")]
    private static extern ref readonly TokenBucketRateLimiterOptions PrivateGetOptions(TokenBucketRateLimiter rateLimiter);
}