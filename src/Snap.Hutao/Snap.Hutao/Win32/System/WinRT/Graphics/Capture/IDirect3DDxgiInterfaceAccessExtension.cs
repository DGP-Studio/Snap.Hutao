// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using WinRT;

namespace Snap.Hutao.Win32.System.WinRT.Graphics.Capture;

internal static class IDirect3DDxgiInterfaceAccessExtension
{
    public static unsafe HRESULT GetInterface<TVftbl>(this IDirect3DDxgiInterfaceAccess access, ref readonly Guid iid, out ObjectReference<TVftbl> i)
        where TVftbl : unmanaged
    {
        fixed (Guid* riid = &iid)
        {
            TVftbl** p = default;
            HRESULT hr = access.GetInterface(riid, (void**)&p);
            i = ObjectReference<TVftbl>.Attach(ref *(nint*)&p, iid);
            return hr;
        }
    }
}