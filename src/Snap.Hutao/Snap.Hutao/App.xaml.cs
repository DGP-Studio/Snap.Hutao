// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Extension;
using Snap.Hutao.Service.Metadata;
using System.Diagnostics;
using Windows.Storage;
using Windows.UI.ViewManagement;

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

        // Notice that we already call InitializeDependencyInjection() above
        // so we can use Ioc here.
        logger = Ioc.Default.GetRequiredService<ILogger<App>>();

        _ = new ExceptionRecorder(this, logger);
    }

    /// <summary>
    /// 当前窗口
    /// </summary>
    public static Window? Window { get => window; set => window = value; }

    /// <inheritdoc cref="Application"/>
    public static new App Current
    {
        get => (App)Application.Current;
    }

    /// <inheritdoc cref="ApplicationData.Current.TemporaryFolder"/>
    public static StorageFolder CacheFolder
    {
        get => ApplicationData.Current.TemporaryFolder;
    }

    /// <inheritdoc cref="ApplicationData.Current.LocalSettings"/>
    public static ApplicationDataContainer Settings
    {
        get => ApplicationData.Current.LocalSettings;
    }

    /// <inheritdoc/>
    [SuppressMessage("", "VSTHRD100")]
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        AppActivationArguments activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        AppInstance firstInstance = AppInstance.FindOrRegisterForKey("main");

        if (firstInstance.IsCurrent)
        {
            Activation.Activate(firstInstance, activatedEventArgs);
            firstInstance.Activated += Activation.Activate;

            logger.LogInformation(EventIds.CommonLog, "Cache folder : {folder}", CacheFolder.Path);

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

    private static void InitializeDependencyInjection()
    {
        IServiceProvider services = new ServiceCollection()

            // Microsoft extension
            .AddLogging(builder => builder
                .AddDebug()
                .AddDatabase())
            .AddMemoryCache()

            // Hutao extensions
            .AddJsonSerializerOptions()
            .AddDatebase()
            .AddInjections()
            .AddHttpClients()

            // Discrete services
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
            .AddSingleton(new UISettings())

            .BuildServiceProvider();

        Ioc.Default.ConfigureServices(services);
    }
}
