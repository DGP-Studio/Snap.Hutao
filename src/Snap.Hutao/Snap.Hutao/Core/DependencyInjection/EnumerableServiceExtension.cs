// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Snap.Hutao.Core.DependencyInjection.Abstraction;

namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// 服务集合扩展
/// </summary>
internal static class EnumerableServiceExtension
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

    /// <summary>
    /// 选择对应的服务
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="isOversea">是否为海外服/Hoyolab</param>
    /// <returns>对应的服务</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TService Pick<TService>(this IEnumerable<TService> services, bool isOversea)
        where TService : IOverseaSupport
    {
        return services.Single(s => s.IsOversea == isOversea);
    }

    /// <summary>
    /// 选择对应的服务
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="isOversea">是否为海外服/Hoyolab</param>
    /// <returns>对应的服务</returns>
    public static TService PickRequiredService<TService>(this IServiceProvider serviceProvider, bool isOversea)
        where TService : IOverseaSupport
    {
        return serviceProvider.GetRequiredService<IEnumerable<TService>>().Pick(isOversea);
    }
}