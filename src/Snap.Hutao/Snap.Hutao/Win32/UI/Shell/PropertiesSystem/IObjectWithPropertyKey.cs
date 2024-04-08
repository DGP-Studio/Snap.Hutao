// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.UI.Shell.PropertiesSystem;

[SupportedOSPlatform("windows6.0.6000")]
[Guid("FC0CA0A7-C316-4FD2-9031-3E628E6D4F23")]
internal unsafe struct IObjectWithPropertyKey
{
    public readonly Vftbl* ThisPtr;

    internal static unsafe ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xA7, 0xA0, 0x0C, 0xFC, 0x16, 0xC3, 0xD2, 0x4F, 0x90, 0x31, 0x3E, 0x62, 0x8E, 0x6D, 0x4F, 0x23];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    public unsafe HRESULT QueryInterface<TInterface>(ref readonly Guid riid, out TInterface* pvObject)
        where TInterface : unmanaged
    {
        fixed (Guid* riid2 = &riid)
        {
            fixed (TInterface** ppvObject = &pvObject)
            {
                return ThisPtr->IUnknownVftbl.QueryInterface((IUnknown*)Unsafe.AsPointer(ref this), riid2, (void**)ppvObject);
            }
        }
    }

    public uint AddRef()
    {
        return ThisPtr->IUnknownVftbl.AddRef((IUnknown*)Unsafe.AsPointer(ref this));
    }

    public uint Release()
    {
        return ThisPtr->IUnknownVftbl.Release((IUnknown*)Unsafe.AsPointer(ref this));
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknown.Vftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IObjectWithPropertyKey*, PROPERTYKEY*, HRESULT> SetPropertyKey;
        internal readonly delegate* unmanaged[Stdcall]<IObjectWithPropertyKey*, PROPERTYKEY*, HRESULT> GetPropertyKey;
    }
}