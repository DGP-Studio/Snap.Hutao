// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows5.0")]
internal unsafe struct IShellLinkDataList
{
    internal static Guid IID = new(1172485294U, 45507, 4560, 185, 47, 0, 160, 201, 3, 18, 225);

    private Vftbl* thisPtr;

    public unsafe HRESULT QueryInterface<TInterface>(ref readonly Guid riid, out TInterface* pvObject)
        where TInterface : unmanaged
    {
        fixed (Guid* riid2 = &riid)
        {
            fixed (TInterface** ppvObject = &pvObject)
            {
                return thisPtr->IUnknownVftbl.QueryInterface((IUnknown*)Unsafe.AsPointer(ref this), riid2, (void**)ppvObject);
            }
        }
    }

    public uint AddRef()
    {
        return thisPtr->IUnknownVftbl.AddRef((IUnknown*)Unsafe.AsPointer(ref this));
    }

    public uint Release()
    {
        return thisPtr->IUnknownVftbl.Release((IUnknown*)Unsafe.AsPointer(ref this));
    }

    public HRESULT GetFlags(out uint dwFlags)
    {
        fixed (uint* pdwFlags = &dwFlags)
        {
            return thisPtr->GetFlags((IShellLinkDataList*)Unsafe.AsPointer(ref this), pdwFlags);
        }
    }

    public HRESULT SetFlags(uint dwFlags)
    {
        return thisPtr->SetFlags((IShellLinkDataList*)Unsafe.AsPointer(ref this), dwFlags);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknown.Vftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkDataList*, void*, HRESULT> AddDataBlock;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkDataList*, uint, void**, HRESULT> CopyDataBlock;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkDataList*, uint, HRESULT> RemoveDataBlock;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkDataList*, uint*, HRESULT> GetFlags;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkDataList*, uint, HRESULT> SetFlags;
    }
}