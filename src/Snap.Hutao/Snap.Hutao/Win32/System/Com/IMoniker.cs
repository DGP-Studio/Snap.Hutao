// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.System.Com;

[SupportedOSPlatform("windows5.0")]
[Guid("0000000F-0000-0000-C000-000000000046")]
internal unsafe struct IMoniker
{
    public readonly Vftbl* ThisPtr;

    internal static unsafe ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x0F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46];
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
                return ThisPtr->IPersistStreamVftbl.IPersistVftbl.IUnknownVftbl.QueryInterface((IUnknown*)Unsafe.AsPointer(ref this), riid2, (void**)ppvObject);
            }
        }
    }

    public uint AddRef()
    {
        return ThisPtr->IPersistStreamVftbl.IPersistVftbl.IUnknownVftbl.AddRef((IUnknown*)Unsafe.AsPointer(ref this));
    }

    public uint Release()
    {
        return ThisPtr->IPersistStreamVftbl.IPersistVftbl.IUnknownVftbl.Release((IUnknown*)Unsafe.AsPointer(ref this));
    }

    internal readonly struct Vftbl
    {
        internal readonly IPersistStream.Vftbl IPersistStreamVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IBindCtx*, IMoniker*, Guid*, void**, HRESULT> BindToObject;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IBindCtx*, IMoniker*, Guid*, void**, HRESULT> BindToStorage;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IBindCtx*, uint, IMoniker**, IMoniker**, HRESULT> Reduce;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IMoniker*, BOOL, IMoniker**, HRESULT> ComposeWith;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, BOOL, IEnumMoniker**, HRESULT> Enum;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IMoniker*, HRESULT> IsEqual;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, uint*, HRESULT> Hash;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IBindCtx*, IMoniker*, IMoniker*, HRESULT> IsRunning;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IBindCtx*, IMoniker*, FILETIME*, HRESULT> GetTimeOfLastChange;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IMoniker**, HRESULT> Inverse;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IMoniker*, IMoniker**, HRESULT> CommonPrefixWith;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IMoniker*, IMoniker**, HRESULT> RelativePathTo;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IBindCtx*, IMoniker*, PWSTR*, HRESULT> GetDisplayName;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IBindCtx*, IMoniker*, PWSTR, uint*, IMoniker**, HRESULT> ParseDisplayName;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, uint*, HRESULT> IsSystemMoniker;
    }
}
