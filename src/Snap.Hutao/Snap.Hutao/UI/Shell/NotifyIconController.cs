// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32;
using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.Graphics;
using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using static Snap.Hutao.Win32.ConstValues;

namespace Snap.Hutao.UI.Shell;

[Injection(InjectAs.Singleton)]
internal sealed partial class NotifyIconController : IDisposable
{
    private readonly LazySlim<NotifyIconContextMenu> lazyMenu;
    private readonly NotifyIconXamlHostWindow xamlHostWindow;
    private readonly NotifyIconMessageWindow messageWindow;
    private readonly System.Drawing.Icon icon;
    private readonly string registryKey;
    private readonly Guid id;

    public NotifyIconController(IServiceProvider serviceProvider)
    {
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

        registryKey = InitializeNotifyIconRegistryKey();
    }

    public void Dispose()
    {
        messageWindow.Dispose();
        NotifyIconMethods.Delete(id);
        icon.Dispose();
    }

    public RECT GetRect()
    {
        return NotifyIconMethods.GetRect(id, messageWindow.HWND);
    }

    public bool GetIsPromoted()
    {
        return Registry.GetValue(registryKey, "IsPromoted", 0) is 1;
    }

    private static string InitializeNotifyIconRegistryKey()
    {
        if (!UniversalApiContract.IsPresent(WindowsVersion.Windows11Version24H2))
        {
            return string.Empty;
        }

        using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Control Panel\NotifyIconSettings"))
        {
            ArgumentNullException.ThrowIfNull(key);
            foreach (ref readonly string subKeyName in key.GetSubKeyNames().AsSpan())
            {
                using (RegistryKey? subKey = key.OpenSubKey(subKeyName))
                {
                    if (subKey?.GetValue("ExecutablePath") is not string executablePath)
                    {
                        continue;
                    }

                    if (executablePath.Equals(InstalledLocation.GetAbsolutePath("Snap.Hutao.exe"), StringComparison.OrdinalIgnoreCase))
                    {
                        return $@"HKEY_CURRENT_USER\Control Panel\NotifyIconSettings\{subKeyName}";
                    }
                }
            }
        }

        throw HutaoException.NotSupported();
    }

    private void OnRecreateNotifyIconRequested(NotifyIconMessageWindow window)
    {
        NotifyIconMethods.Delete(id);
        if (!NotifyIconMethods.Add(id, window.HWND, "Snap Hutao", NotifyIconMessageWindow.WM_NOTIFYICON_CALLBACK, icon.Handle))
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
        if (!NotifyIconMethods.Add(id, messageWindow.HWND, "Snap Hutao", NotifyIconMessageWindow.WM_NOTIFYICON_CALLBACK, icon.Handle))
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
            return;
        }

        xamlHostWindow.ShowFlyoutAt(lazyMenu.Value, new Windows.Foundation.Point(point.X, point.Y), GetRect());
    }
}