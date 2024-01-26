// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.System.Com;

[SupportedOSPlatform("windows5.0")]
[Guid("0000000E-0000-0000-C000-000000000046")]
internal unsafe struct IBindCtx
{
    public readonly Vftbl* ThisPtr;

    internal static unsafe ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46];
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
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, IUnknown*, HRESULT> RegisterObjectBound;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, IUnknown*, HRESULT> RevokeObjectBound;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, HRESULT> ReleaseBoundObjects;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, BIND_OPTS*, HRESULT> SetBindOptions;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, BIND_OPTS*, HRESULT> GetBindOptions;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, IRunningObjectTable**, HRESULT> GetRunningObjectTable;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, PWSTR, IUnknown*, HRESULT> RegisterObjectParam;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, PWSTR, IUnknown**, HRESULT> GetObjectParam;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, IEnumString**, HRESULT> EnumObjectParam;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, PWSTR, HRESULT> RevokeObjectParam;
    }
}