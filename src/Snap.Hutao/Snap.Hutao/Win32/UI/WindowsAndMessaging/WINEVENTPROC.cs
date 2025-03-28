// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.UI.WindowsAndMessaging;

internal readonly unsafe struct WINEVENTPROC
{
    private readonly delegate* unmanaged[Stdcall]<HWINEVENTHOOK, WINEVENT_ID, HWND, int, int, uint, uint, void> value;

    public WINEVENTPROC(delegate* unmanaged[Stdcall]<HWINEVENTHOOK, WINEVENT_ID, HWND, int, int, uint, uint, void> method)
    {
        value = method;
    }

    public static WINEVENTPROC Create(delegate* unmanaged[Stdcall]<HWINEVENTHOOK, WINEVENT_ID, HWND, int, int, uint, uint, void> method)
    {
        return new(method);
    }
}