// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;

namespace Snap.Hutao.Extension;

/// <summary>
/// 内存缓存拓展
/// </summary>
internal static class MemoryCacheExtension
{
    public static bool TryRemove(this IMemoryCache memoryCache, string key, out object? value)
    {
        if (!memoryCache.TryGetValue(key, out value))
        {
            return false;
        }

        memoryCache.Remove(key);
        return true;
    }

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