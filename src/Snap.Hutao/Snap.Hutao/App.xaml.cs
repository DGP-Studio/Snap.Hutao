// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle;
using System.Diagnostics;
using Windows.Storage;

namespace Snap.Hutao;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// This class must be public
/// </summary>
[HighQuality]
[Injection(InjectAs.Singleton)]
[SuppressMessage("", "SH001")]
public sealed partial class App : Application
{
    private const string AppInstanceKey = "main";
    private readonly ILogger<App> logger;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes the singleton application object.
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public App(IServiceProvider serviceProvider)
    {
        // Load app resource
        InitializeComponent();

        logger = serviceProvider.GetRequiredService<ILogger<App>>();
        serviceProvider.GetRequiredService<ExceptionRecorder>().Record(this);

        this.serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            AppActivationArguments activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            AppInstance firstInstance = AppInstance.FindOrRegisterForKey(AppInstanceKey);

            if (firstInstance.IsCurrent)
            {
                // manually invoke
                Activation.NonRedirectToActivate(firstInstance, activatedEventArgs);
                firstInstance.Activated += Activation.Activate;
                ToastNotificationManagerCompat.OnActivated += Activation.NotificationActivate;

                LogDiagnosticInformation();
                PostLaunchAsync().SafeForget(logger);
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

    private static async Task PostLaunchAsync()
    {
        await JumpListHelper.ConfigureAsync().ConfigureAwait(false);
    }

    private void LogDiagnosticInformation()
    {
        HutaoOptions hutaoOptions = serviceProvider.GetRequiredService<HutaoOptions>();

        logger.LogInformation("Snap Hutao FamilyName: {name}", hutaoOptions.FamilyName);
        logger.LogInformation("Snap Hutao Version: {version}", hutaoOptions.Version);
        logger.LogInformation("Snap Hutao LocalCache: {folder}", hutaoOptions.LocalCache);
    }
}