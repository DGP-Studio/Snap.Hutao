// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;

namespace Snap.Hutao.Extension;

internal static class MemoryCacheExtension
{
    public static bool TryGetRequiredValue<T>(this IMemoryCache memoryCache, string key, [NotNullWhen(true)] out T? value)
        where T : class
    {
        if (!memoryCache.TryGetValue(key, out value))
        {
            return false;
        }

        ArgumentNullException.ThrowIfNull(value);
        return true;
    }
}