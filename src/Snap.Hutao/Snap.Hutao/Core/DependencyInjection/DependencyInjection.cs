// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Windows.ApplicationModel.Resources;
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
            .AddLogging(builder => builder.AddUnconditionalDebug())
            .AddMemoryCache()

            // Hutao extensions
            .AddJsonOptions()
            .AddDatabase()
            .AddInjections()
            .AddHttpClients()

            // Discrete services
            .AddSingleton<IMessenger, WeakReferenceMessenger>()

            .BuildServiceProvider(true);

        Ioc.Default.ConfigureServices(serviceProvider);

        serviceProvider.InitializeCulture();

        return serviceProvider;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InitializeCulture(this IServiceProvider serviceProvider)
    {
        AppOptions appOptions = serviceProvider.GetRequiredService<AppOptions>();
        appOptions.PreviousCulture = CultureInfo.CurrentCulture;

        CultureInfo cultureInfo = appOptions.CurrentCulture;

        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
        ApplicationLanguages.PrimaryLanguageOverride = cultureInfo.Name;
    }
}