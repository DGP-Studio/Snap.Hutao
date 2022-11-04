// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Exception;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Extension;
using Snap.Hutao.Service.Metadata;
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
    /// <param name="appCenter">App Center</param>
    public App(ILogger<App> logger)
    {
        // load app resource
        InitializeComponent();
        this.logger = logger;

        _ = new ExceptionRecorder(this, logger);
    }

    /// <inheritdoc/>
    [SuppressMessage("", "VSTHRD100")]
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        AppActivationArguments activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        AppInstance firstInstance = AppInstance.FindOrRegisterForKey("main");

        if (firstInstance.IsCurrent)
        {
            // manually invoke
            Activation.Activate(firstInstance, activatedEventArgs);
            firstInstance.Activated += Activation.Activate;

            logger.LogInformation(EventIds.CommonLog, "Snap Hutao : {version}", CoreEnvironment.Version);
            logger.LogInformation(EventIds.CommonLog, "Cache folder : {folder}", ApplicationData.Current.TemporaryFolder.Path);

            JumpListHelper.ConfigAsync().SafeForget(logger);

            Ioc.Default
                .GetRequiredService<IMetadataService>()
                .ImplictAs<IMetadataInitializer>()?
                .InitializeInternalAsync()
                .SafeForget(logger);
        }
        else
        {
            // Redirect the activation (and args) to the "main" instance, and exit.
            await firstInstance.RedirectActivationToAsync(activatedEventArgs);
            Process.GetCurrentProcess().Kill();
        }
    }
}