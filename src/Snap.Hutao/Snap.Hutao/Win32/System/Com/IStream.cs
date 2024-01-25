// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Win32.System.Com;

internal unsafe struct IStream
{
    internal static Guid IID = new(12u, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

    private Vftbl* thisPtr;

    public unsafe HRESULT QueryInterface<TInterface>(ref readonly Guid riid, out TInterface* pvObject)
        where TInterface : unmanaged
    {
        fixed (Guid* riid2 = &riid)
        {
            fixed (TInterface** ppvObject = &pvObject)
            {
                return thisPtr->ISequentialStreamVftbl.IUnknownVftbl.QueryInterface((IUnknown*)Unsafe.AsPointer(ref this), riid2, (void**)ppvObject);
            }
        }
    }

    public uint AddRef()
    {
        return thisPtr->ISequentialStreamVftbl.IUnknownVftbl.AddRef((IUnknown*)Unsafe.AsPointer(ref this));
    }

    public uint Release()
    {
        return thisPtr->ISequentialStreamVftbl.IUnknownVftbl.Release((IUnknown*)Unsafe.AsPointer(ref this));
    }

    internal readonly struct Vftbl
    {
        internal readonly ISequentialStream.Vftbl ISequentialStreamVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, long, STREAM_SEEK, ulong*, HRESULT> Seek;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, ulong, HRESULT> SetSize;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, IStream*, ulong, ulong*, ulong*, HRESULT> CopyTo;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, uint, HRESULT> Commit;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, HRESULT> Revert;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, ulong, ulong, uint, HRESULT> LockRegion;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, ulong, ulong, uint, HRESULT> UnlockRegion;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, STATSTG*, uint, HRESULT> Stat;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, IStream**, HRESULT> Clone;
    }
}