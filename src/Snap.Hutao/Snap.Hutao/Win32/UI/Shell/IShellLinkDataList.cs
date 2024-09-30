// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT;
using WinRT.Interop;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows5.0")]
internal static unsafe class IShellLinkDataList
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xAE, 0xB4, 0xE2, 0x45, 0xC3, 0xB1, 0xD0, 0x11, 0xB9, 0x2F, 0x00, 0xA0, 0xC9, 0x03, 0x12, 0xE1]);
    }

    public static HRESULT GetFlags(this ObjectReference<Vftbl> objRef, out uint dwFlags)
    {
        fixed (uint* pdwFlags = &dwFlags)
        {
            return objRef.Vftbl.GetFlags(objRef.ThisPtr, pdwFlags);
        }
    }

    public static HRESULT SetFlags(this ObjectReference<Vftbl> objRef, uint dwFlags)
    {
        return objRef.Vftbl.SetFlags(objRef.ThisPtr, dwFlags);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, void*, HRESULT> AddDataBlock;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, void**, HRESULT> CopyDataBlock;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, HRESULT> RemoveDataBlock;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint*, HRESULT> GetFlags;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, HRESULT> SetFlags;
    }
}