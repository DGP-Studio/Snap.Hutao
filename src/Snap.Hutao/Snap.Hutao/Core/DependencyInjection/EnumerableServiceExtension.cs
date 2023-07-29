// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using System.Runtime.CompilerServices;

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
    /// <param name="isOversea">是否为海外服/Hoyolab</param>
    /// <returns>对应的服务</returns>
    [Obsolete("该方法会导致不必要的服务实例化")]
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
    [Obsolete("该方法会导致不必要的服务实例化")]
    public static TService PickRequiredService<TService>(this IServiceProvider serviceProvider, bool isOversea)
        where TService : IOverseaSupport
    {
        return serviceProvider.GetRequiredService<IEnumerable<TService>>().Pick(isOversea);
    }
}