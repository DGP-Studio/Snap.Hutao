// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;

namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// 依赖注入
/// </summary>
internal static class DependencyInjection
{
    /// <summary>
    /// 初始化依赖注入
    /// </summary>
    /// <returns>服务提供器</returns>
    public static ServiceProvider Initialize()
    {
        ServiceProvider serviceProvider = new ServiceCollection()

            // Microsoft extension
            .AddLogging(builder => builder.AddDebug())
            .AddMemoryCache()

            // Hutao extensions
            .AddJsonOptions()
            .AddDatabase()
            .AddInjections()
            .AddHttpClients()

            // Discrete services
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)

            .BuildServiceProvider(true);

        Ioc.Default.ConfigureServices(serviceProvider);
        return serviceProvider;
    }
}