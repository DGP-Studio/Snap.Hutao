// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.SystemServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows5.1.2600")]
internal static unsafe class IShellItem
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x1E, 0x6D, 0x82, 0x43, 0x18, 0xE7, 0xEE, 0x42, 0xBC, 0x55, 0xA1, 0xE2, 0x61, 0xC3, 0x7B, 0xFE]);
    }

    public static HRESULT GetDisplayName(this ObjectReference<Vftbl> objRef, SIGDN sigdnName, out PWSTR pszName)
    {
        fixed (PWSTR* ppszName = &pszName)
        {
            return objRef.Vftbl.GetDisplayName(objRef.ThisPtr, sigdnName, ppszName);
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, Guid*, Guid*, void**, HRESULT> BindToHandler;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, HRESULT> GetParent;
        internal readonly delegate* unmanaged[Stdcall]<nint, SIGDN, PWSTR*, HRESULT> GetDisplayName;
        internal readonly delegate* unmanaged[Stdcall]<nint, SFGAO_FLAGS, SFGAO_FLAGS*, HRESULT> GetAttributes;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, uint, int*, HRESULT> Compare;
    }
}