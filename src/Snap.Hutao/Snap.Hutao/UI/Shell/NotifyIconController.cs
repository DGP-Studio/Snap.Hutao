// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.Win32;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.Factory.ContentDialog;
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
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly LazySlim<NotifyIconContextMenu> lazyMenu;
    private readonly NotifyIconXamlHostWindow xamlHostWindow;
    private readonly NotifyIconMessageWindow messageWindow;
    private readonly System.Drawing.Icon icon;
    private readonly string registryKey;
    private readonly Guid id;

    public NotifyIconController(IServiceProvider serviceProvider)
    {
        contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();
        lazyMenu = new(() => new(serviceProvider));

        string iconPath = InstalledLocation.GetAbsolutePath("Assets/Logo.ico");

        icon = new(iconPath);
        id = MemoryMarshal.AsRef<Guid>(MD5.HashData(Encoding.UTF8.GetBytes(iconPath)).AsSpan());

        xamlHostWindow = new(serviceProvider);
        xamlHostWindow.MoveAndResize(default);

        messageWindow = new()
        {
            TaskbarCreated = OnRecreateNotifyIconRequested,
            ContextMenuRequested = OnContextMenuRequested,
            IconSelected = OnContextMenuRequested,
        };

        CreateNotifyIcon();

        registryKey = InitializeNotifyIconRegistryKey(id);
    }

    public static Lock InitializationSyncRoot { get; } = new();

    public static bool IsPromoted(IServiceProvider serviceProvider)
    {
        try
        {
            NotifyIconController notifyIconController = serviceProvider.LockAndGetRequiredService<NotifyIconController>(NotifyIconController.InitializationSyncRoot);

            // Actual version should be above 24H2 (26100), which is 26120 without UniversalApiContract.
            if (Core.UniversalApiContract.IsPresent(WindowsVersion.Windows11Version24H2))
            {
                return notifyIconController.GetIsPromoted();
            }

            // Shell_NotifyIconGetRect can return E_FAIL in multiple cases.
            RECT iconRect = notifyIconController.GetRect();
            if (Core.UniversalApiContract.IsPresent(WindowsVersion.Windows11))
            {
                RECT primaryRect = DisplayArea.Primary.OuterBounds.ToRECT();
                return IntersectRect(out _, in primaryRect, in iconRect);
            }

            HWND shellTrayWnd = FindWindowExW(default, default, "Shell_TrayWnd", default);
            HWND trayNotifyWnd = FindWindowExW(shellTrayWnd, default, "TrayNotifyWnd", default);
            HWND button = FindWindowExW(trayNotifyWnd, default, "Button", default);

            if (GetWindowRect(button, out RECT buttonRect))
            {
                return !EqualRect(in buttonRect, in iconRect);
            }

            return false;
        }
        catch
        {
#if DEBUG
            // Check your explorer, did you modify your default taskbar?
            throw;
#else
            return false;
#endif
        }
    }

    public void Dispose()
    {
        messageWindow.Dispose();
        NotifyIconMethods.Delete(id);
        icon.Dispose();
    }

    public RECT GetRect()
    {
        return NotifyIconMethods.GetRect(id, messageWindow.Hwnd);
    }

    private static string InitializeNotifyIconRegistryKey(Guid id)
    {
        if (!UniversalApiContract.IsPresent(WindowsVersion.Windows11Version24H2))
        {
            return string.Empty;
        }

        // The GUID is stored in the registry as a string in the format {xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}
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

        throw HutaoException.NotSupported();
    }

    private bool GetIsPromoted()
    {
        return Registry.GetValue(registryKey, "IsPromoted", 0) is 1;
    }

    private void OnRecreateNotifyIconRequested(NotifyIconMessageWindow window)
    {
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

        xamlHostWindow.ShowFlyoutAt(lazyMenu.Value, new Windows.Foundation.Point(point.X, point.Y), GetRect());
    }
}