// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Shell;
using System.Diagnostics;

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
    private readonly IServiceProvider serviceProvider;
    private readonly IActivation activation;
    private readonly ILogger<App> logger;

    /// <summary>
    /// Initializes the singleton application object.
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public App(IServiceProvider serviceProvider)
    {
        // Load app resource
        InitializeComponent();

        activation = serviceProvider.GetRequiredService<IActivation>();
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
                activation.NonRedirectToActivate(firstInstance, activatedEventArgs);
                activation.InitializeWith(firstInstance);

                LogDiagnosticInformation();
                serviceProvider.GetRequiredService<IJumpListInterop>().ConfigureAsync().SafeForget();
            }
            else
            {
                // Redirect the activation (and args) to the "main" instance, and exit.
                firstInstance.RedirectActivationTo(activatedEventArgs);
                Process.GetCurrentProcess().Kill();
            }
        }
        catch
        {
            // AppInstance.GetCurrent() calls failed
            Process.GetCurrentProcess().Kill();
        }
    }

    private void LogDiagnosticInformation()
    {
        HutaoOptions hutaoOptions = serviceProvider.GetRequiredService<HutaoOptions>();

        logger.LogInformation("FamilyName: {name}", hutaoOptions.FamilyName);
        logger.LogInformation("Version: {version}", hutaoOptions.Version);
        logger.LogInformation("LocalCache: {folder}", hutaoOptions.LocalCache);
    }
}