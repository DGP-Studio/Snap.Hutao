﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Storage;
using static Snap.Hutao.Win32.ConstValues;

namespace Snap.Hutao.Core.Windowing.NotifyIcon;

[Injection(InjectAs.Singleton)]
internal sealed class NotifyIconController : IDisposable
{
    private readonly LazySlim<NotifyIconContextMenu> lazyMenu = new(() => new());
    private readonly NotifyIconXamlHostWindow xamlHostWindow;
    private readonly NotifyIconMessageWindow messageWindow;
    private readonly System.Drawing.Icon icon;

    public NotifyIconController()
    {
        StorageFile iconFile = StorageFile.GetFileFromApplicationUriAsync("ms-appx:///Assets/Logo.ico".ToUri()).AsTask().GetAwaiter().GetResult();
        icon = new(iconFile.Path);

        xamlHostWindow = new();

        messageWindow = new()
        {
            TaskbarCreated = window =>
            {
                NotifyIconMethods.Delete(Id);
                if (!NotifyIconMethods.Add(Id, window.HWND, "Snap Hutao", NotifyIconMessageWindow.WM_NOTIFYICON_CALLBACK, (HICON)icon.Handle))
                {
                    HutaoException.InvalidOperation("Failed to recreate NotifyIcon");
                }
            },
            ContextMenuRequested = (window, point) =>
            {
                Flyout flyout = lazyMenu.Value;
                RECT iconRect = NotifyIconMethods.GetRect(Id, window.HWND);
                xamlHostWindow.ShowFlyoutAt(flyout, new Windows.Foundation.Point(point.X, point.Y), iconRect);
            },
        };

        NotifyIconMethods.Delete(Id);
        if (!NotifyIconMethods.Add(Id, messageWindow.HWND, "Snap Hutao", NotifyIconMessageWindow.WM_NOTIFYICON_CALLBACK, (HICON)icon.Handle))
        {
            HutaoException.InvalidOperation("Failed to create NotifyIcon");
        }

        if (!NotifyIconMethods.SetVersion(Id, NOTIFYICON_VERSION_4))
        {
            HutaoException.InvalidOperation("Failed to set NotifyIcon version");
        }
    }

    private static ref readonly Guid Id
    {
        get
        {
            // MD5 for "Snap.Hutao"
            ReadOnlySpan<byte> data = [0xEE, 0x01, 0x5C, 0xCB, 0xF3, 0x97, 0xC6, 0x93, 0xE8, 0x77, 0xCE, 0x09, 0x54, 0x90, 0xEE, 0xAC];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    public void Dispose()
    {
        messageWindow.Dispose();
        NotifyIconMethods.Delete(Id);
        icon.Dispose();

        xamlHostWindow.Dispose();
    }
}