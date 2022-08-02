// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Extension;

/// <summary>
/// 对象扩展
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// <see langword="as"/> 的链式调用扩展
    /// </summary>
    /// <typeparam name="T">目标转换类型</typeparam>
    /// <param name="obj">对象</param>
    /// <returns>转换类型后的对象</returns>
    public static T? ImplictAs<T>(this object? obj)
        where T : class
    {
        return obj as T;
    }
}