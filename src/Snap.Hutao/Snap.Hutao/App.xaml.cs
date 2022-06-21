// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

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
            .AddLogging(builder => builder.AddDebug())
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
            .AddMemoryCache()
            .AddDatebase()
            .AddHttpClients()
            .AddDefaultJsonSerializerOptions()
            .AddInjections()
            .BuildServiceProvider();

        Ioc.Default.ConfigureServices(services);
    }
}
