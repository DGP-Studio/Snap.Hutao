// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.System.Com;

internal static class IUnknownMarshal
{
    public static unsafe HRESULT QueryInterface<TInterface>(void* pIUnknown, ref readonly Guid riid, out TInterface* pvObject)
        where TInterface : unmanaged
    {
        fixed (Guid* riid2 = &riid)
        {
            fixed (TInterface** ppvObject = &pvObject)
            {
                return ((IUnknown*)pIUnknown)->ThisPtr->QueryInterface((IUnknown*)pIUnknown, riid2, (void**)ppvObject);
            }
        }
    }

    public static unsafe uint AddRef(void* pIUnknown)
    {
        return ((IUnknown*)pIUnknown)->ThisPtr->AddRef((IUnknown*)pIUnknown);
    }

    public static unsafe uint Release(void* pIUnknown)
    {
        return ((IUnknown*)pIUnknown)->ThisPtr->Release((IUnknown*)pIUnknown);
    }
}