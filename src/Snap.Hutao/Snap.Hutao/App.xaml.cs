// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Logging;

namespace Snap.Hutao;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private static Window? window;

    /// <summary>
    /// Initializes the singleton application object.
    /// This is the first line of authored code executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        // load app resource
        InitializeComponent();
        InitializeDependencyInjection();

        UnhandledException += AppUnhandledException;
    }

    /// <summary>
    /// 当前窗口
    /// </summary>
    public static Window? Window { get => window; set => window = value; }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.
    /// Other entry points will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Window = Ioc.Default.GetRequiredService<MainWindow>();
        Window.Activate();
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
            .AddDefaultJsonSerializerOptions()

            // Discrete services
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)

            .BuildServiceProvider();

        Ioc.Default.ConfigureServices(services);
    }

    private void AppUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        ILogger<App> logger = Ioc.Default.GetRequiredService<ILogger<App>>();
        logger.LogError(EventIds.UnhandledException, e.Exception, "未经处理的异常");
    }
}
