// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Logging;
using System.Diagnostics;
using Windows.Storage;

namespace Snap.Hutao;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
[Injection(InjectAs.Singleton)]
public partial class App : Application
{
    private readonly ILogger<App> logger;

    /// <summary>
    /// Initializes the singleton application object.
    /// </summary>
    /// <param name="logger">日志器</param>
    public App(ILogger<App> logger)
    {
        // load app resource
        InitializeComponent();
        this.logger = logger;

        _ = new ExceptionRecorder(this, logger);
    }

    /// <inheritdoc/>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            AppActivationArguments activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            AppInstance firstInstance = AppInstance.FindOrRegisterForKey("main");

            if (firstInstance.IsCurrent)
            {
                // manually invoke
                Activation.NonRedirectToActivate(firstInstance, activatedEventArgs);
                firstInstance.Activated += Activation.Activate;
                ToastNotificationManagerCompat.OnActivated += Activation.NotificationActivate;

                logger.LogInformation("Snap Hutao | {name} : {version}", CoreEnvironment.FamilyName, CoreEnvironment.Version);
                logger.LogInformation("Cache folder : {folder}", ApplicationData.Current.LocalCacheFolder.Path);

                JumpListHelper.ConfigureAsync().SafeForget(logger);
            }
            else
            {
                // Redirect the activation (and args) to the "main" instance, and exit.
                firstInstance.RedirectActivationTo(activatedEventArgs);
                Process.GetCurrentProcess().Kill();
            }
        }
        catch (Exception)
        {
            // AppInstance.GetCurrent() calls failed
            Process.GetCurrentProcess().Kill();
        }
    }
}