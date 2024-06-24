// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Shell;
using Snap.Hutao.Win32.UI.WindowsAndMessaging;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.Shell32;

namespace Snap.Hutao.UI.Shell;

internal sealed class NotifyIconMethods
{
    public static BOOL Add(ref readonly NOTIFYICONDATAW data)
    {
        return Shell_NotifyIconW(NOTIFY_ICON_MESSAGE.NIM_ADD, in data);
    }

    [SuppressMessage("", "SH002")]
    public static unsafe BOOL Add(Guid id, HWND hWnd, string tip, uint uCallbackMessage, HICON hIcon)
    {
        NOTIFYICONDATAW data = default;
        data.cbSize = (uint)sizeof(NOTIFYICONDATAW);
        data.uFlags =
            NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE |
            NOTIFY_ICON_DATA_FLAGS.NIF_ICON |
            NOTIFY_ICON_DATA_FLAGS.NIF_TIP |
            NOTIFY_ICON_DATA_FLAGS.NIF_STATE |
            NOTIFY_ICON_DATA_FLAGS.NIF_GUID;
        data.guidItem = id;
        data.hWnd = hWnd;
        tip.AsSpan().CopyTo(new(data.szTip, 128));
        data.uCallbackMessage = uCallbackMessage;
        data.hIcon = hIcon;
        data.dwStateMask = NOTIFY_ICON_STATE.NIS_HIDDEN;

        return Add(in data);
    }

    public static BOOL Modify(ref readonly NOTIFYICONDATAW data)
    {
        return Shell_NotifyIconW(NOTIFY_ICON_MESSAGE.NIM_MODIFY, in data);
    }

    public static BOOL Delete(ref readonly NOTIFYICONDATAW data)
    {
        return Shell_NotifyIconW(NOTIFY_ICON_MESSAGE.NIM_DELETE, in data);
    }

    public static unsafe BOOL Delete(Guid id)
    {
        NOTIFYICONDATAW data = default;
        data.cbSize = (uint)sizeof(NOTIFYICONDATAW);
        data.uFlags = NOTIFY_ICON_DATA_FLAGS.NIF_GUID;
        data.guidItem = id;

        return Delete(in data);
    }

    [SuppressMessage("", "SH002")]
    public static unsafe RECT GetRect(Guid id, HWND hWND)
    {
        NOTIFYICONIDENTIFIER identifier = new()
        {
            cbSize = (uint)sizeof(NOTIFYICONIDENTIFIER),
            hWnd = hWND,
            guidItem = id,
        };

        Marshal.ThrowExceptionForHR(Shell_NotifyIconGetRect(ref identifier, out RECT rect));
        return rect;
    }

    public static BOOL SetFocus(ref readonly NOTIFYICONDATAW data)
    {
        return Shell_NotifyIconW(NOTIFY_ICON_MESSAGE.NIM_SETFOCUS, in data);
    }

    public static BOOL SetVersion(ref readonly NOTIFYICONDATAW data)
    {
        return Shell_NotifyIconW(NOTIFY_ICON_MESSAGE.NIM_SETVERSION, in data);
    }

    public static unsafe BOOL SetVersion(Guid id, uint version)
    {
        NOTIFYICONDATAW data = default;
        data.cbSize = (uint)sizeof(NOTIFYICONDATAW);
        data.uFlags = NOTIFY_ICON_DATA_FLAGS.NIF_GUID;
        data.guidItem = id;
        data.Anonymous.uVersion = version;

        return SetVersion(in data);
    }
}