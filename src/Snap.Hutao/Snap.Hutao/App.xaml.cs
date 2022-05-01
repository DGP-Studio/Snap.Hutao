// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;

namespace Snap.Hutao;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private Window? mainWindow;

    /// <summary>
    /// Initializes the singleton application object.
    /// This is the first line of authored code executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        // load app resource
        InitializeComponent();

        // prepare DI
        Ioc.Default.ConfigureServices(new ServiceCollection()
            .AddLogging(builder => builder.AddDebug())
            .AddHttpClient()
            .AddInjections(typeof(App))
            .BuildServiceProvider());
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.
    /// Other entry points will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
        mainWindow.Activate();
    }
}
