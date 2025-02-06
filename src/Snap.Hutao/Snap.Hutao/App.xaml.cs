// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.LifeCycle.InterProcess;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.WinUI.FrameworkTheming;
using System.Diagnostics;

namespace Snap.Hutao;

[Injection(InjectAs.Singleton)]
[SuppressMessage("", "SH001", Justification = "The App must be public")]
public sealed partial class App : Application
{
    private const string ConsoleBanner = """
        ----------------------------------------------------------------
          _____                         _    _         _ 
         / ____|                       | |  | |       | |
        | (___   _ __    __ _  _ __    | |__| | _   _ | |_  __ _   ___
         \___ \ | '_ \  / _` || '_ \   |  __  || | | || __|/ _` | / _ \
         ____) || | | || (_| || |_) |_ | |  | || |_| || |_| (_| || (_) |
        |_____/ |_| |_| \__,_|| .__/(_)|_|  |_| \__,_| \__|\__,_| \___/
                              | |
                              |_|
        
        Snap.Hutao is a open source software developed by DGP Studio.
        Copyright (C) 2022 - 2025 DGP Studio, All Rights Reserved.
        ----------------------------------------------------------------
        """;

    private readonly IServiceProvider serviceProvider;
    private readonly IAppActivation activation;
    private readonly ILogger<App> logger;

    public App(IServiceProvider serviceProvider)
    {
        logger = serviceProvider.GetRequiredService<ILogger<App>>();
        Gen2GcCallback.Register(() =>
        {
            logger.LogDebug("Gen2 GC is triggered.");
            return true;
        });

        // Load app resource
        InitializeComponent();
        logger.LogDebug("Application Component initialized.");

        ExceptionHandlingSupport.Initialize(serviceProvider, this);
        logger.LogDebug("Exception handling initialized.");

        activation = serviceProvider.GetRequiredService<IAppActivation>();
        this.serviceProvider = serviceProvider;
    }

    public new void Exit()
    {
        XamlApplicationLifetime.Exiting = true;
        logger.LogDebug("Application exiting.");
        base.Exit();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            // Important: You must call AppNotificationManager::Default().Register
            // before calling AppInstance.GetCurrent.GetActivatedEventArgs.
            AppNotificationManager.Default.NotificationInvoked += activation.NotificationInvoked;
            AppNotificationManager.Default.Register();
            logger.LogDebug("\e[1m\e[32mAppNotification\e[37m registered.");

            AppActivationArguments activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();

            Bootstrap.UseNamedPipeRedirection();
            if (serviceProvider.GetRequiredService<PrivateNamedPipeClient>().TryRedirectActivationTo(activatedEventArgs))
            {
                logger.LogDebug("\e[1m\e[32mApplication\e[37m exit on \e[1m\e[33mRedirectActivationTo\e[37m");
                Exit();
                return;
            }

            logger.LogInformation($"\e[33m{ConsoleBanner}\e[37m");
            logger.LogInformation("\e[1m\e[34mFamilyName: \e[1m\e[36m{Name}\e[37m", HutaoRuntime.FamilyName);
            logger.LogInformation("\e[1m\e[34mVersion: \e[1m\e[36m{Version}\e[37m", HutaoRuntime.Version);
            logger.LogInformation("\e[1m\e[34mLocalCache: \e[1m\e[36m{Path}\e[37m", HutaoRuntime.LocalCache);

            FrameworkTheming.SetTheme(ThemeHelper.ElementToFramework(serviceProvider.GetRequiredService<AppOptions>().ElementTheme));

            // Manually invoke
            activation.ActivateAndInitialize(HutaoActivationArguments.FromAppActivationArguments(activatedEventArgs));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Application failed in App.OnLaunched");
            Process.GetCurrentProcess().Kill();
        }
    }
}