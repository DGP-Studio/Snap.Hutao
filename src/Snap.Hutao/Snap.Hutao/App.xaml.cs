// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.LifeCycle.InterProcess;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service;
using Snap.Hutao.UI.Shell;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.UI.Xaml.Control.Theme;
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

        // Load app resource
        InitializeComponent();

        ExceptionHandlingSupport.Initialize(serviceProvider, this);

        activation = serviceProvider.GetRequiredService<IAppActivation>();
        this.serviceProvider = serviceProvider;
    }

    public new void Exit()
    {
        XamlApplicationLifetime.Exiting = true;
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateInfo("Application exiting", "Hutao"));

        if (XamlApplicationLifetime.LaunchedWithNotifyIcon)
        {
            Window window = serviceProvider.GetRequiredService<NotifyIconController>().XamlHost;
            SpinWait.SpinUntil(() => VisualTreeHelper.GetOpenPopups(window).Count <= 0);
        }

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

            AppActivationArguments activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();

            Bootstrap.UseNamedPipeRedirection();
            if (serviceProvider.GetRequiredService<PrivateNamedPipeClient>().TryRedirectActivationTo(activatedEventArgs))
            {
                SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateInfo("Application exiting on RedirectActivationTo", "Hutao"));
                Exit();
                return;
            }

            logger.LogInformation($"\e[33m{ConsoleBanner}\e[37m");

            FrameworkTheming.SetTheme(ThemeHelper.ElementToFramework(serviceProvider.GetRequiredService<AppOptions>().ElementTheme));

            // Manually invoke
            SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateInfo("Activate and Initialize", "Application"));
            activation.ActivateAndInitialize(HutaoActivationArguments.FromAppActivationArguments(activatedEventArgs));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Application failed in App.OnLaunched");
            Process.GetCurrentProcess().Kill();
        }
    }
}