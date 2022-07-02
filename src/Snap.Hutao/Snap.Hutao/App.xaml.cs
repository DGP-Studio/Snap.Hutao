// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service.Abstraction;
using System.Diagnostics;
using Windows.ApplicationModel.Activation;

namespace Snap.Hutao;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private static Window? window;
    private readonly ILogger<App> logger;

    /// <summary>
    /// Initializes the singleton application object.
    /// </summary>
    public App()
    {
        // load app resource
        InitializeComponent();
        InitializeDependencyInjection();

        logger = Ioc.Default.GetRequiredService<ILogger<App>>();
        UnhandledException += AppUnhandledException;
    }

    /// <summary>
    /// 当前窗口
    /// </summary>
    public static Window? Window { get => window; set => window = value; }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    [SuppressMessage("", "VSTHRD100")]
    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        AppActivationArguments activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        AppInstance mainInstance = AppInstance.FindOrRegisterForKey("main");

        if (!mainInstance.IsCurrent)
        {
            // Redirect the activation (and args) to the "main" instance, and exit.
            await mainInstance.RedirectActivationToAsync(activatedEventArgs);
            Process.GetCurrentProcess().Kill();
        }
        else
        {
            Uri? uri = null;
            if (activatedEventArgs.Kind == ExtendedActivationKind.Protocol)
            {
                IProtocolActivatedEventArgs protocolArgs = (activatedEventArgs.Data as IProtocolActivatedEventArgs)!;
                uri = protocolArgs.Uri;
            }

            Window = Ioc.Default.GetRequiredService<MainWindow>();
            Window.Activate();

            if (uri != null)
            {
                Ioc.Default.GetRequiredService<IInfoBarService>().Information(uri.ToString());
            }
        }
    }

    private static void InitializeDependencyInjection()
    {
        IServiceProvider services = new ServiceCollection()

            // Microsoft extension
            .AddLogging(builder => builder.AddDebug())
            .AddMemoryCache()

            // Hutao extensions
            .AddInjections()
            .AddDatebase()
            .AddHttpClients()
            .AddJsonSerializerOptions()

            // Discrete services
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)

            .BuildServiceProvider();

        Ioc.Default.ConfigureServices(services);
    }

    private void AppUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        logger.LogError(EventIds.UnhandledException, e.Exception, "未经处理的异常");
    }
}
