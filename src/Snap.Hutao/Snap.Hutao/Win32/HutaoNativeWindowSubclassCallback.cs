// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32;

internal readonly unsafe struct HutaoNativeWindowSubclassCallback
{
    private readonly delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nint, LRESULT*, BOOL> value;

    public HutaoNativeWindowSubclassCallback(delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nint, LRESULT*, BOOL> value)
    {
        this.value = value;
    }

    public static HutaoNativeWindowSubclassCallback Create(delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, nint, LRESULT*, BOOL> method)
    {
        return new(method);
    }
}