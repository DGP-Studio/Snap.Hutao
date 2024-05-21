// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.UI.Shell;

internal unsafe readonly struct SUBCLASSPROC
{
    private readonly delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nuint, nuint, LRESULT> value;

    public SUBCLASSPROC(delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nuint, nuint, LRESULT> method)
    {
        value = method;
    }

    public static SUBCLASSPROC Create(delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nuint, nuint, LRESULT> method)
    {
        return new(method);
    }
}