// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.Windows.Globalization;
using Quartz;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Response;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.DependencyInjection;

internal static class DependencyInjection
{
    public static ServiceProvider Initialize()
    {
        IServiceCollection services = new ServiceCollection()

            // Microsoft extension
            .AddLogging(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddFilter(DbLoggerCategory.Database.Command.Name, level => level >= LogLevel.Information)
                    .AddFilter(DbLoggerCategory.Query.Name, level => level >= LogLevel.Information)
                    .AddDebug()
                    .AddSentryTelemetry();
            })
            .AddMemoryCache()

            // Quartz
            .AddQuartz()

            // Hutao extensions
            .AddJsonOptions()
            .AddDatabase()
            .AddServices()
            .AddResponseValidation()
            .AddConfiguredHttpClients()

            // Discrete services
            .AddSingleton<IMessenger, WeakReferenceMessenger>();

        ServiceProvider serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });

        Ioc.Default.ConfigureServices(serviceProvider);

        serviceProvider.InitializeCulture();
        serviceProvider.InitializeNotification();

        return serviceProvider;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InitializeCulture(this IServiceProvider serviceProvider)
    {
        CultureOptions cultureOptions = serviceProvider.GetRequiredService<CultureOptions>();
        cultureOptions.SystemCulture = CultureInfo.CurrentCulture;

        CultureInfo cultureInfo = cultureOptions.CurrentCulture.Value;

        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;

        try
        {
            ApplicationLanguages.PrimaryLanguageOverride = cultureInfo.Name;
        }
        catch (COMException)
        {
            // 0x80070032 ERROR_NOT_SUPPORTED
        }

        SH.Culture = cultureInfo;
        XamlApplicationLifetime.CultureInfoInitialized = true;

        ILogger<CultureOptions> logger = serviceProvider.GetRequiredService<ILogger<CultureOptions>>();
        logger.LogDebug("System Culture: {System}", cultureOptions.SystemCulture);
        logger.LogDebug("Current Culture: {Current}", cultureInfo);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void InitializeNotification(this IServiceProvider serviceProvider)
    {
        _ = serviceProvider.GetRequiredService<IAppNotificationLifeTime>();
    }
}