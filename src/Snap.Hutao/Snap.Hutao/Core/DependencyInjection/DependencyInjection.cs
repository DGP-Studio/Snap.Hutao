// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Http;
using Snap.Hutao.Core.IO.Http.Proxy;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service;
using System.Globalization;
using System.Net.Http;
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
            .AddLogging(builder => builder.AddDebug().AddConsoleWindow())
            .AddMemoryCache()

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

        CultureInfo cultureInfo = cultureOptions.CurrentCulture;

        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;

        ApplicationLanguages.PrimaryLanguageOverride = cultureInfo.Name;

        SH.Culture = cultureInfo;
    }

    private static void InitializeConsoleWindow(this IServiceProvider serviceProvider)
    {
        _ = serviceProvider.GetRequiredService<ConsoleWindowLifeTime>();
    }
}