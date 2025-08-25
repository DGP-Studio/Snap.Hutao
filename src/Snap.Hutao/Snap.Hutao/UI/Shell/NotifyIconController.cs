// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.UI.Windowing;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Snap.Hutao.UI.Shell;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class NotifyIconController : IDisposable
{
    private static bool constructed;

    private readonly Lock syncRoot = new();

    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly LazySlim<NotifyIconContextMenu> lazyMenu;
    private readonly NotifyIconXamlHostWindow xamlHostWindow;
    private readonly IServiceProvider serviceProvider;
    private readonly HutaoNativeNotifyIcon native;
    private readonly nint handle;

    private bool disposed;

    public NotifyIconController(IServiceProvider serviceProvider)
    {
        if (Interlocked.Exchange(ref constructed, true))
        {
            // Actively prevent multiple constructions, if this happens, it's definitely a bug.
            // For example: the below part of the ctor throws an exception.
            throw new InvalidOperationException("NotifyIconController is already constructed.");
        }

        currentXamlWindowReference = serviceProvider.GetRequiredService<ICurrentXamlWindowReference>();
        contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();
        this.serviceProvider = serviceProvider;
        lazyMenu = new(() => new(serviceProvider));

        string iconPath = InstalledLocation.GetAbsolutePath("Assets/Logo.ico");
        Guid id = MemoryMarshal.AsRef<Guid>(MD5.HashData(Encoding.UTF8.GetBytes(iconPath)).AsSpan());
        native = HutaoNative.Instance.MakeNotifyIcon(iconPath, in id);

        xamlHostWindow = new(serviceProvider);
        xamlHostWindow.MoveAndResize(default);

        handle = GCHandle.ToIntPtr(GCHandle.Alloc(this));
    }

    public static Lock InitializationSyncRoot { get; } = new();

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        lock (syncRoot)
        {
            disposed = true;
            try
            {
                native.Destroy();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }

            GCHandle.FromIntPtr(handle).Free();
        }
    }

    public unsafe void Create()
    {
        native.Create(HutaoNativeNotifyIconCallback.Create(&OnNotifyIconCallback), handle, "Snap Hutao");
    }

    public bool IsPromoted()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        try
        {
            return native.IsPromoted();
        }
        catch (Exception ex)
        {
            // If the lpValue registry value does not exist, the function returns ERROR_FILE_NOT_FOUND
            if (ex is not (FileNotFoundException or COMException or ObjectDisposedException))
            {
                SentrySdk.CaptureException(ex);
            }

            return false;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void OnNotifyIconCallback(HutaoNativeNotifyIconCallbackKind kind, RECT icon, POINT point, nint data)
    {
        if (GCHandle.FromIntPtr(data).Target is not NotifyIconController controller)
        {
            return;
        }

        switch (kind)
        {
            case HutaoNativeNotifyIconCallbackKind.TaskbarCreated:
                controller.OnRecreateNotifyIconRequested();
                break;
            case HutaoNativeNotifyIconCallbackKind.ContextMenu:
            case HutaoNativeNotifyIconCallbackKind.LeftButtonDown:
                controller.OnContextMenuRequested(icon, point);
                break;
            case HutaoNativeNotifyIconCallbackKind.LeftButtonDoubleClick:
                controller.OnWindowRequested();
                break;
        }
    }

    private void OnRecreateNotifyIconRequested()
    {
        if (disposed || XamlApplicationLifetime.Exiting)
        {
            return;
        }

        native.Recreate("Snap Hutao");
    }

    private void OnContextMenuRequested(RECT icon, POINT point)
    {
        if (disposed)
        {
            return;
        }

        if (XamlApplicationLifetime.Exiting)
        {
            Debugger.Break();
            return;
        }

        // https://github.com/DGP-Studio/Snap.Hutao/issues/2434
        // Now we disable the context menu when the dialog is showing.
        if (contentDialogFactory.IsDialogShowing)
        {
            return;
        }

        xamlHostWindow.ShowFlyoutAt(lazyMenu.Value, new(point.x, point.y), icon);
    }

    private void OnWindowRequested()
    {
        if (disposed)
        {
            return;
        }

        if (XamlApplicationLifetime.Exiting)
        {
            Debugger.Break();
            return;
        }

        switch (currentXamlWindowReference.Window)
        {
            case null:
                {
                    // MainWindow is closed, show it
                    MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                    currentXamlWindowReference.Window = mainWindow;
                    mainWindow.SwitchTo();
                    mainWindow.AppWindow.MoveInZOrderAtTop();
                    return;
                }

            default:
                {
                    Window window = currentXamlWindowReference.Window;

                    // While window is closing, currentXamlWindowReference can still retrieve the window,
                    // just ignore it
                    if (window.AppWindow is not null)
                    {
                        window.SwitchTo();
                        window.AppWindow.MoveInZOrderAtTop();
                    }

                    return;
                }
        }
    }
}