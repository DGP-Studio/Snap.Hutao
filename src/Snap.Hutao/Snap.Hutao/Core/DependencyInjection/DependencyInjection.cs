// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Quartz;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service;
using System.Globalization;
using System.Runtime.CompilerServices;
using Windows.Globalization;

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
            .AddLogging(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddDebug()
                    .AddConsoleWindow();
            })
            .AddMemoryCache()

            // Quartz
            .AddQuartz()

            // Hutao extensions
            .AddJsonOptions()
            .AddDatabase()
            .AddInjections()
            .AddAllHttpClients()

            // Discrete services
            .AddSingleton<IMessenger, WeakReferenceMessenger>()
            .BuildServiceProvider(true);

        Ioc.Default.ConfigureServices(serviceProvider);

        serviceProvider.InitializeConsoleWindow();
        serviceProvider.InitializeCulture();

        return serviceProvider;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InitializeCulture(this IServiceProvider serviceProvider)
    {
        CultureOptions cultureOptions = serviceProvider.GetRequiredService<CultureOptions>();
        cultureOptions.SystemCulture = CultureInfo.CurrentCulture;

        ILogger<CultureOptions> logger = serviceProvider.GetRequiredService<ILogger<CultureOptions>>();
        logger.LogDebug("System Culture: {System}", cultureOptions.SystemCulture);

        CultureInfo cultureInfo = cultureOptions.CurrentCulture;

        logger.LogDebug("Current Culture: {Current}", cultureInfo);

        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;

        ApplicationLanguages.PrimaryLanguageOverride = cultureInfo.Name;

        SH.Culture = cultureInfo;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InitializeConsoleWindow(this IServiceProvider serviceProvider)
    {
        _ = serviceProvider.GetRequiredService<ConsoleWindowLifeTime>();
    }
}