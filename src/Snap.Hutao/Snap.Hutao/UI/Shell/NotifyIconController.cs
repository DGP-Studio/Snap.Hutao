// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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