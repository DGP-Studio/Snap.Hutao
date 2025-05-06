// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Win32;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.UI.Xaml;
using Snap.Hutao.UI.Xaml.View.Window;
using Snap.Hutao.Win32.Foundation;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using static Snap.Hutao.Win32.ConstValues;
using static Snap.Hutao.Win32.User32;

namespace Snap.Hutao.UI.Shell;

[Injection(InjectAs.Singleton)]
internal sealed partial class NotifyIconController : IDisposable
{
    private static bool constructed;

    private readonly Lock syncRoot = new();

    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly LazySlim<NotifyIconContextMenu> lazyMenu;
    private readonly NotifyIconXamlHostWindow xamlHostWindow;
    private readonly NotifyIconMessageWindow messageWindow;
    private readonly IServiceProvider serviceProvider;
    private readonly System.Drawing.Icon icon;
    private readonly string? registryKey;
    private readonly Guid id;

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

        icon = new(iconPath);
        id = MemoryMarshal.AsRef<Guid>(MD5.HashData(Encoding.UTF8.GetBytes(iconPath)).AsSpan());

        xamlHostWindow = new(serviceProvider);
        xamlHostWindow.MoveAndResize(default);

        messageWindow = new()
        {
            TaskbarCreated = OnRecreateNotifyIconRequested,
            MainWindowRequested = OnMainWindowRequested,
            ContextMenuRequested = OnContextMenuRequested,
            IconSelected = OnContextMenuRequested,
        };

        CreateNotifyIcon();

        try
        {
            registryKey = InitializeNotifyIconRegistryKey(id);
        }
        catch (HutaoException)
        {
        }
    }

    public static Lock InitializationSyncRoot { get; } = new();

    public Microsoft.UI.Xaml.Window XamlHost { get => xamlHostWindow; }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        lock (syncRoot)
        {
            disposed = true;

            messageWindow.Dispose();
            NotifyIconMethods.Delete(id);
            icon.Dispose();
        }
    }

    public RECT GetRect()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        return NotifyIconMethods.GetRect(id, messageWindow.Hwnd);
    }

    public bool GetIsPromoted()
    {
        if (string.IsNullOrEmpty(registryKey))
        {
            // If the registry key is not available, we assume that the icon is not promoted.
            return false;
        }

        ObjectDisposedException.ThrowIf(disposed, this);
        return Registry.GetValue(registryKey, "IsPromoted", 0) is 1;
    }

    private static string InitializeNotifyIconRegistryKey(Guid id)
    {
        if (!UniversalApiContract.IsPresent(WindowsVersion.Windows11Version24H2))
        {
            return string.Empty;
        }

        // The GUID is stored in the registry as a REG_SZ in the format {xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}
        string idString = id.ToString("B");
        using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Control Panel\NotifyIconSettings"))
        {
            ArgumentNullException.ThrowIfNull(key);
            foreach (ref readonly string subKeyName in key.GetSubKeyNames().AsSpan())
            {
                using (RegistryKey? subKey = key.OpenSubKey(subKeyName))
                {
                    if (subKey?.GetValue("IconGuid") is not string iconGuid)
                    {
                        continue;
                    }

                    if (string.Equals(iconGuid, idString, StringComparison.OrdinalIgnoreCase))
                    {
                        return $@"HKEY_CURRENT_USER\Control Panel\NotifyIconSettings\{subKeyName}";
                    }
                }
            }
        }

        throw HutaoException.NotSupported("Unable to find NotifyIcon registry key", HutaoException.Marker);
    }

    private void OnRecreateNotifyIconRequested(NotifyIconMessageWindow window)
    {
        if (disposed)
        {
            return;
        }

        NotifyIconMethods.Delete(id);
        if (!NotifyIconMethods.Add(id, window.Hwnd, "Snap Hutao", NotifyIconMessageWindow.WM_NOTIFYICON_CALLBACK, icon.Handle))
        {
            HutaoException.InvalidOperation("Failed to recreate NotifyIcon");
        }

        if (!NotifyIconMethods.SetVersion(id, NOTIFYICON_VERSION_4))
        {
            HutaoException.InvalidOperation("Failed to set NotifyIcon version");
        }
    }

    private void CreateNotifyIcon()
    {
        if (disposed)
        {
            return;
        }

        NotifyIconMethods.Delete(id);
        if (!NotifyIconMethods.Add(id, messageWindow.Hwnd, "Snap Hutao", NotifyIconMessageWindow.WM_NOTIFYICON_CALLBACK, icon.Handle))
        {
            HutaoException.InvalidOperation("Failed to create NotifyIcon");
        }

        if (!NotifyIconMethods.SetVersion(id, NOTIFYICON_VERSION_4))
        {
            HutaoException.InvalidOperation("Failed to set NotifyIcon version");
        }
    }

    private void OnContextMenuRequested(NotifyIconMessageWindow window, PointUInt16 point)
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

        RECT rect;
        try
        {
            rect = GetRect();
        }
        catch (Exception)
        {
            // Fallback to the mouse position
            GetCursorPos(out POINT pos);
            rect = new(pos.x - 8, pos.y - 8, pos.x + 8, pos.y + 8);
        }

        xamlHostWindow.ShowFlyoutAt(lazyMenu.Value, new(point.X, point.Y), rect);
    }

    private void OnMainWindowRequested(NotifyIconMessageWindow window)
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
            case MainWindow mainWindow:
            {
                // While window is closing, currentXamlWindowReference can still retrieve the window,
                // just ignore it
                if (mainWindow.AppWindow is not null)
                {
                    // MainWindow is activated, bring to foreground
                    mainWindow.SwitchTo();
                    mainWindow.AppWindow.MoveInZOrderAtTop();
                }

                return;
            }

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
                Window otherWindow = currentXamlWindowReference.Window;
                otherWindow.SwitchTo();
                otherWindow.AppWindow.MoveInZOrderAtTop();
                return;
            }
        }
    }
}