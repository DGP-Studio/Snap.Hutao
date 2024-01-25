// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using Snap.Hutao.Win32.System.SystemServices;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Win32.UI.Shell;

internal unsafe struct IShellItem
{
    internal static Guid IID = new(1132621086u, 59160, 17134, 188, 85, 161, 226, 97, 195, 123, 254);

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

    public HRESULT GetDisplayName(SIGDN sigdnName, out PWSTR pszName)
    {
        fixed (PWSTR* ppszName = &pszName)
        {
            return thisPtr->GetDisplayName((IShellItem*)Unsafe.AsPointer(ref this), sigdnName, ppszName);
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknown.Vftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IShellItem*, IBindCtx*, Guid*, Guid*, void**, HRESULT> BindToHandler;
        internal readonly delegate* unmanaged[Stdcall]<IShellItem*, IShellItem**, HRESULT> GetParent;
        internal readonly delegate* unmanaged[Stdcall]<IShellItem*, SIGDN, PWSTR*, HRESULT> GetDisplayName;
        internal readonly delegate* unmanaged[Stdcall]<IShellItem*, SFGAO_FLAGS, SFGAO_FLAGS*, HRESULT> GetAttributes;
        internal readonly delegate* unmanaged[Stdcall]<IShellItem*, IShellItem*, uint, int*, HRESULT> Compare;
    }
}