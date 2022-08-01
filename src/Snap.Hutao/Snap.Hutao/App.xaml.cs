// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.VisualStudio.Threading;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Extension;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Metadata;
using System.Diagnostics;
using Windows.Storage;

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
        AppInstance.GetCurrent().Activated += OnActivated;
        // load app resource
        InitializeComponent();
        InitializeDependencyInjection();

        // Notice that we already call InitializeDependencyInjection() above
        // so we can use Ioc here.
        logger = Ioc.Default.GetRequiredService<ILogger<App>>();

        UnhandledException += AppUnhandledException;
        DebugSettings.BindingFailed += XamlBindingFailed;
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

    /// <summary>
    /// <inheritdoc cref="ApplicationData.Current.TemporaryFolder"/>
    /// </summary>
    public static StorageFolder CacheFolder
    {
        get => ApplicationData.Current.TemporaryFolder;
    }

    /// <summary>
    /// <inheritdoc cref="ApplicationData.Current.LocalSettings"/>
    /// </summary>
    public static ApplicationDataContainer Settings
    {
        get => ApplicationData.Current.LocalSettings;
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// Any async operation in this method should be wrapped with try catch
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    [SuppressMessage("", "VSTHRD100")]
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        AppActivationArguments activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        AppInstance firstInstance = AppInstance.FindOrRegisterForKey("main");

        if (!firstInstance.IsCurrent)
        {
            // Redirect the activation (and args) to the "main" instance, and exit.
            await firstInstance.RedirectActivationToAsync(activatedEventArgs);
            Process.GetCurrentProcess().Kill();
        }
        else
        {
            Window = Ioc.Default.GetRequiredService<MainWindow>();
            Window.Activate();

            logger.LogInformation(EventIds.CommonLog, "Cache folder : {folder}", CacheFolder.Path);

            Ioc.Default
                .GetRequiredService<IMetadataService>()
                .As<IMetadataInitializer>()?
                .InitializeInternalAsync()
                .SafeForget(logger);
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
            .AddInjections()
            .AddDatebase()
            .AddHttpClients()
            .AddJsonSerializerOptions()

            // Discrete services
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)

            .BuildServiceProvider();

        Ioc.Default.ConfigureServices(services);
    }

    private void OnActivated(object? sender, AppActivationArguments args)
    {
        if (args.Kind == ExtendedActivationKind.Protocol)
        {
            if (args.TryGetProtocolActivatedUri(out Uri? uri))
            {
                Ioc.Default.GetRequiredService<IInfoBarService>().Information(uri.ToString());
            }
        }
    }

    private void AppUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        logger.LogError(EventIds.UnhandledException, e.Exception, "未经处理的异常: [HResult:{code}]", e.Exception.HResult);
    }

    private void XamlBindingFailed(object sender, BindingFailedEventArgs e)
    {
        logger.LogCritical(EventIds.XamlBindingError, "XAML绑定失败: {message}", e.Message);
    }
}
