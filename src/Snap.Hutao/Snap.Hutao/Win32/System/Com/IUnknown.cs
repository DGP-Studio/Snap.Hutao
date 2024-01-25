// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Win32.System.Com;

internal unsafe struct IUnknown
{
    internal static Guid IID = new(0u, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

    private Vftbl* thisPtr;

    public unsafe HRESULT QueryInterface<TInterface>(ref readonly Guid riid, out TInterface* pvObject)
        where TInterface : unmanaged
    {
        fixed (Guid* riid2 = &riid)
        {
            fixed (TInterface** ppvObject = &pvObject)
            {
                return thisPtr->QueryInterface((IUnknown*)Unsafe.AsPointer(ref this), riid2, (void**)ppvObject);
            }
        }
    }

    public uint AddRef()
    {
        return thisPtr->AddRef((IUnknown*)Unsafe.AsPointer(ref this));
    }

    public uint Release()
    {
        return thisPtr->Release((IUnknown*)Unsafe.AsPointer(ref this));
    }

    internal unsafe readonly struct Vftbl
    {
        internal readonly delegate* unmanaged[Stdcall]<IUnknown*, Guid*, void**, HRESULT> QueryInterface;
        internal readonly delegate* unmanaged[Stdcall]<IUnknown*, uint> AddRef;
        internal readonly delegate* unmanaged[Stdcall]<IUnknown*, uint> Release;
    }
}