// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;

namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// 命名服务扩展
/// </summary>
internal static class NamedServiceExtension
{
    /// <summary>
    /// 选择对应的服务
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="name">名称</param>
    /// <returns>对应的服务</returns>
    public static TService Pick<TService>(this IEnumerable<TService> services, string name)
        where TService : INamedService
    {
        return services.Single(s => s.Name == name);
    }
}