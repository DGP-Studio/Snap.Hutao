// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.UI.WindowsAndMessaging;

internal readonly unsafe struct HOOKPROC
{
    [SuppressMessage("", "IDE0052")]
    private readonly delegate* unmanaged[Stdcall]<int, WPARAM, LPARAM, LRESULT> value;

    public HOOKPROC(delegate* unmanaged[Stdcall]<int, WPARAM, LPARAM, LRESULT> method)
    {
        value = method;
    }

    public static HOOKPROC Create(delegate* unmanaged[Stdcall]<int, WPARAM, LPARAM, LRESULT> method)
    {
        return new(method);
    }
}