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

        UnhandledException += AppUnhandledException;
        DebugSettings.BindingFailed += XamlBindingFailed;
        TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
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
            firstInstance.Activated += OnActivated;
            Window = Ioc.Default.GetRequiredService<MainWindow>();

            logger.LogInformation(EventIds.CommonLog, "Cache folder : {folder}", CacheFolder.Path);

            OnActivated(firstInstance, activatedEventArgs);

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
            .AddInjections()
            .AddHttpClients()
            .AddDatebase()
            .AddJsonSerializerOptions()

            // Discrete services
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
            .AddSingleton(new UISettings())

            .BuildServiceProvider();

        Ioc.Default.ConfigureServices(services);
    }

    [SuppressMessage("", "VSTHRD100")]
    private async void OnActivated(object? sender, AppActivationArguments args)
    {
        IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();
        await infoBarService.WaitInitializationAsync();
        infoBarService.Information("OnActivated");

        if (args.Kind == ExtendedActivationKind.Protocol)
        {
            if (args.TryGetProtocolActivatedUri(out Uri? uri))
            {
                infoBarService.Information(uri.ToString());
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

    private void TaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        logger.LogCritical(EventIds.UnobservedTaskException, "异步任务执行异常: {message}", e.Exception);
    }
}
