// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Core.DependencyInjection.Annotation;
using Snap.Hutao.Core.Validation;
using Snap.Hutao.Extension;

namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// 服务管理器
/// 依赖注入的核心管理类
/// </summary>
internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// 向容器注册服务, 调用 <see cref="Register(IServiceCollection, Type)"/>
    /// </summary>
    /// <param name="services">容器</param>
    /// <param name="entryType">入口类型，该类型所在的程序集均会被扫描</param>
    /// <returns>可继续操作的服务集合</returns>
    public static IServiceCollection AddInjections(this IServiceCollection services, Type entryType)
    {
        entryType.Assembly.ForEachType(type => Register(services, type));
        return services;
    }

    /// <summary>
    /// 向容器注册类型
    /// </summary>
    /// <param name="services">容器</param>
    /// <param name="type">待检测的类型</param>
    /// <returns>可继续操作的服务集合</returns>
    public static IServiceCollection Register(this IServiceCollection services, Type type)
    {
        if (type.TryGetAttribute(out InjectionAttribute? attr))
        {
            if (attr.InterfaceType is not null)
            {
                return attr.InjectAs switch
                {
                    InjectAs.Singleton => services.AddSingleton(attr.InterfaceType, type),
                    InjectAs.Transient => services.AddTransient(attr.InterfaceType, type),
                    _ => Must.NeverHappen<IServiceCollection>(),
                };
            }
            else
            {
                return attr.InjectAs switch
                {
                    InjectAs.Singleton => services.AddSingleton(type),
                    InjectAs.Transient => services.AddTransient(type),
                    _ => throw Must.NeverHappen(),
                };
            }
        }

        return services;
    }
}
