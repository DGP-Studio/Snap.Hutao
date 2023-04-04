// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;

namespace Snap.Hutao.Extension;

/// <summary>
/// 内存缓存拓展
/// </summary>
internal static class MemoryCacheExtension
{
    /// <summary>
    /// 尝试从 IMemoryCache 中移除并返回具有指定键的值
    /// </summary>
    /// <param name="memoryCache">缓存</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns>是否移除成功</returns>
    public static bool TryRemove(this IMemoryCache memoryCache, string key, [NotNullWhen(true)] out object? value)
    {
        if (memoryCache.TryGetValue(key, out value))
        {
            memoryCache.Remove(key);
            return true;
        }

        return false;
    }
}