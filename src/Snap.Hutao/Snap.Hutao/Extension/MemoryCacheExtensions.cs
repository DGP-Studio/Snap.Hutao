// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Extension;

/// <summary>
/// 内存缓存扩展
/// </summary>
public static class MemoryCacheExtensions
{
    /// <summary>
    /// 获取缓存键名称
    /// </summary>
    /// <param name="className">类名</param>
    /// <param name="propertyName">属性名</param>
    /// <returns>缓存</returns>
    public static string GetCacheKey(string className, string propertyName)
    {
        return $"{className}.Cache.{propertyName}";
    }
}