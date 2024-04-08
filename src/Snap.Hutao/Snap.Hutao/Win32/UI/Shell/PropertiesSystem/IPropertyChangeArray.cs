// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.UI.Shell.PropertiesSystem;

[SupportedOSPlatform("windows6.0.6000")]
[Guid("380F5CAD-1B5E-42F2-805D-637FD392D31E")]
internal unsafe struct IPropertyChangeArray
{
    public readonly Vftbl* ThisPtr;

    internal static unsafe ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xAD, 0x5C, 0x0F, 0x38, 0x5E, 0x1B, 0xF2, 0x42, 0x80, 0x5D, 0x63, 0x7F, 0xD3, 0x92, 0xD3, 0x1E];
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
        internal readonly delegate* unmanaged[Stdcall]<IPropertyChangeArray*, uint*, HRESULT> GetCount;
        internal readonly delegate* unmanaged[Stdcall]<IPropertyChangeArray*, uint, Guid*, void**, HRESULT> GetAt;
        internal readonly delegate* unmanaged[Stdcall]<IPropertyChangeArray*, uint, IPropertyChange*, HRESULT> InsertAt;
        internal readonly delegate* unmanaged[Stdcall]<IPropertyChangeArray*, IPropertyChange*, HRESULT> Append;
        internal readonly delegate* unmanaged[Stdcall]<IPropertyChangeArray*, IPropertyChange*, HRESULT> AppendOrReplace;
        internal readonly delegate* unmanaged[Stdcall]<IPropertyChangeArray*, uint, HRESULT> RemoveAt;
        internal readonly delegate* unmanaged[Stdcall]<IPropertyChangeArray*, PROPERTYKEY*, HRESULT> IsKeyInArray;
    }
}