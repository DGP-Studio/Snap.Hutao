// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// 服务管理器
/// 依赖注入的核心管理类
/// </summary>
[HighQuality]
internal static partial class ServiceCollectionExtension
{
    /// <summary>
    /// 向容器注册服务
    /// 此方法将会自动生成
    /// </summary>
    /// <param name="services">容器</param>
    /// <returns>可继续操作的服务集合</returns>
    public static partial IServiceCollection AddInjections(this IServiceCollection services);
}