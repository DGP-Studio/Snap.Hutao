// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// 对象扩展
/// </summary>
[HighQuality]
internal static class CastableServiceExtension
{
    /// <summary>
    /// <see langword="as"/> 的链式调用扩展
    /// </summary>
    /// <typeparam name="T">目标转换类型</typeparam>
    /// <param name="service">对象</param>
    /// <returns>转换类型后的对象</returns>
    public static T? As<T>(this ICastableService service)
        where T : class
    {
        return service as T;
    }
}