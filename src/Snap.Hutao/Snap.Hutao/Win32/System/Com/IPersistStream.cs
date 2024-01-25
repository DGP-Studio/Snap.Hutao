// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Win32.System.Com;

internal unsafe struct IPersistStream
{
    internal static Guid IID = new(265u, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

    private Vftbl* thisPtr;

    public unsafe HRESULT QueryInterface<TInterface>(ref readonly Guid riid, out TInterface* pvObject)
        where TInterface : unmanaged
    {
        fixed (Guid* riid2 = &riid)
        {
            fixed (TInterface** ppvObject = &pvObject)
            {
                return thisPtr->IPersistVftbl.IUnknownVftbl.QueryInterface((IUnknown*)Unsafe.AsPointer(ref this), riid2, (void**)ppvObject);
            }
        }
    }

    public uint AddRef()
    {
        return thisPtr->IPersistVftbl.IUnknownVftbl.AddRef((IUnknown*)Unsafe.AsPointer(ref this));
    }

    public uint Release()
    {
        return thisPtr->IPersistVftbl.IUnknownVftbl.Release((IUnknown*)Unsafe.AsPointer(ref this));
    }

    internal readonly struct Vftbl
    {
        internal readonly IPersist.Vftbl IPersistVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IPersistStream*, HRESULT> IsDirty;
        internal readonly delegate* unmanaged[Stdcall]<IPersistStream*, IStream*, HRESULT> Load;
        internal readonly delegate* unmanaged[Stdcall]<IPersistStream*, IStream*, BOOL, HRESULT> Save;
        internal readonly delegate* unmanaged[Stdcall]<IPersistStream*, ulong*, HRESULT> GetSizeMax;
    }
}
