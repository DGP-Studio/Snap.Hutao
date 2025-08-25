// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.LifeCycle.InterProcess;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.UI.Xaml.Control.Theme;
using System.Diagnostics;

namespace Snap.Hutao;

[ConstructorGenerated(InitializeComponent = true)]
[Service(ServiceLifetime.Singleton)]
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

    /// <summary>
    /// Shortcut to get the <see cref="AppOptions"/> instance.
    /// </summary>
    internal partial AppOptions Options { get; }

    partial void PostConstruct(IServiceProvider serviceProvider)
    {
        ExceptionHandlingSupport.Initialize(serviceProvider, this);
    }

    [SuppressMessage("", "SA1202")]
    public new void Exit()
    {
        XamlApplicationLifetime.Exiting = true;
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateInfo("Application exiting", "Hutao"));
        SpinWait.SpinUntil(static () => XamlApplicationLifetime.ActivationAndInitializationCompleted);
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

            // E_INVALIDARG E_OUTOFMEMORY
            AppActivationArguments activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();

            if (serviceProvider.GetRequiredService<PrivateNamedPipeClient>().TryRedirectActivationTo(activatedEventArgs))
            {
                SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateInfo("Application exiting on RedirectActivationTo", "Hutao"));
                XamlApplicationLifetime.ActivationAndInitializationCompleted = true;
                Exit();
                return;
            }

            logger.LogInformation($"{ConsoleBanner}");

            FrameworkTheming.SetTheme(ThemeHelper.ElementToFramework(serviceProvider.GetRequiredService<AppOptions>().ElementTheme));

            // Manually invoke
            SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateInfo("Activate and Initialize", "Application"));
            activation.ActivateAndInitialize(HutaoActivationArguments.FromAppActivationArguments(activatedEventArgs));
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            SentrySdk.Flush();

            Process.GetCurrentProcess().Kill();
        }
    }
}