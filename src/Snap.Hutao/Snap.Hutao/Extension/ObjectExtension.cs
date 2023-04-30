// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Extension;

/// <summary>
/// 对象拓展
/// </summary>
internal static class ObjectExtension
{
    /// <summary>
    /// 转换到只有1长度的数组
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="source">源</param>
    /// <returns>数组</returns>
    public static T[] ToArray<T>(this T source)
    {
        return new T[] { source };
    }
}