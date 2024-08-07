// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Threading.RateLimiting;

namespace Snap.Hutao.Core.Threading.RateLimiting;

internal static class TokenBucketRateLimiterExtension
{
    // IMPORTANT: acquired can be none 0 values if false is returned
    public static bool TryAcquire(this TokenBucketRateLimiter rateLimiter, int permits, out int acquired, out TimeSpan retryAfter)
    {
        acquired = Math.Min(permits, (int)Volatile.Read(ref PrivateGetTokenCount(rateLimiter)));
        return rateLimiter.AttemptAcquire(acquired).TryGetMetadata(MetadataName.RetryAfter, out retryAfter);
    }

    public static void Replenish(this TokenBucketRateLimiter rateLimiter, int permits)
    {
        if (permits <= 0)
        {
            return;
        }

        ref double tokenCount = ref PrivateGetTokenCount(rateLimiter);
        Volatile.Write(ref tokenCount, Math.Min(PrivateGetOptions(rateLimiter).TokenLimit, Volatile.Read(ref tokenCount) + permits));
    }

    // private double _tokenCount;
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_tokenCount")]
    private static extern ref double PrivateGetTokenCount(TokenBucketRateLimiter rateLimiter);

    // private readonly TokenBucketRateLimiterOptions _options;
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_options")]
    private static extern ref readonly TokenBucketRateLimiterOptions PrivateGetOptions(TokenBucketRateLimiter rateLimiter);
}