// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.UI.WindowsAndMessaging;

internal readonly unsafe struct WNDPROC
{
    [SuppressMessage("", "IDE0052")]
    private readonly delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, LRESULT> value;

    public WNDPROC(delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, LRESULT> method)
    {
        value = method;
    }

    public static WNDPROC Create(delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, LRESULT> method)
    {
        return new(method);
    }
}