// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.System.WinRT.Graphics.Capture;

internal static class IDirect3DDxgiInterfaceAccessExtension
{
    public static unsafe HRESULT GetInterface<T>(this IDirect3DDxgiInterfaceAccess access, ref readonly Guid iid, out T* p)
        where T : unmanaged
    {
        fixed (Guid* riid = &iid)
        {
            fixed (T** pp = &p)
            {
                return access.GetInterface(riid, (void**)pp);
            }
        }
    }
}