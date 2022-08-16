// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;
using WinRT;

namespace Snap.Hutao;

/// <summary>
/// Program class
/// </summary>
public static class Program
{
    private static volatile DispatcherQueue? dispatcherQueue;

    /// <summary>
    /// 主线程调度器队列
    /// </summary>
    public static DispatcherQueue UIDispatcherQueue
    {
        get => Must.NotNull(dispatcherQueue!);
    }

    [DllImport("Microsoft.ui.xaml.dll")]
    private static extern void XamlCheckProcessRequirements();

    [STAThread]
    private static void Main(string[] args)
    {
        _ = args;
        XamlCheckProcessRequirements();
        ComWrappersSupport.InitializeComWrappers();

        Application.Start(p =>
        {
            dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            SynchronizationContext context = new DispatcherQueueSynchronizationContext(dispatcherQueue);
            SynchronizationContext.SetSynchronizationContext(context);
            _ = new App();
        });
    }
}