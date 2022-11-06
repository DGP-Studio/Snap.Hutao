// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Logging;
using System.Runtime.InteropServices;
using WinRT;

namespace Snap.Hutao;

/// <summary>
/// Program class
/// </summary>
public static partial class Program
{
    /// <summary>
    /// 主线程队列
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("", "SA1401")]
    internal static volatile DispatcherQueue? DispatcherQueue;

    [DllImport("Microsoft.ui.xaml.dll")]
    private static extern void XamlCheckProcessRequirements();

    [STAThread]
    private static void Main(string[] args)
    {
        _ = args;

        XamlCheckProcessRequirements();
        ComWrappersSupport.InitializeComWrappers();

        // by adding the using statement, we can dispose the injected services when we closing
        using (InitializeDependencyInjection())
        {
            // In a Desktop app this runs a message pump internally,
            // and does not return until the application shuts down.
            Application.Start(InitializeApp);
            ServiceScopeExtension.DisposeLast();
        }
    }

    private static void InitializeApp(ApplicationInitializationCallbackParams param)
    {
        DispatcherQueue = DispatcherQueue.GetForCurrentThread();
        DispatcherQueueSynchronizationContext context = new(DispatcherQueue);
        SynchronizationContext.SetSynchronizationContext(context);

        _ = Ioc.Default.GetRequiredService<App>();
    }

    /// <summary>
    /// 初始化依赖注入
    /// </summary>
    /// <returns>The ServiceProvider, so that we can dispose it.</returns>
    private static ServiceProvider InitializeDependencyInjection()
    {
        ServiceProvider services = new ServiceCollection()

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

            .BuildServiceProvider();

        Ioc.Default.ConfigureServices(services);
        return services;
    }
}