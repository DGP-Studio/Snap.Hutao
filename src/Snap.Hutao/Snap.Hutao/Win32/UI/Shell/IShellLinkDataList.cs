// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows5.0")]
internal unsafe struct IShellLinkDataList
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0xAE, 0xB4, 0xE2, 0x45, 0xC3, 0xB1, 0xD0, 0x11, 0xB9, 0x2F, 0x00, 0xA0, 0xC9, 0x03, 0x12, 0xE1];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    public HRESULT GetFlags(out uint dwFlags)
    {
        fixed (uint* pdwFlags = &dwFlags)
        {
            return ThisPtr->GetFlags((IShellLinkDataList*)Unsafe.AsPointer(ref this), pdwFlags);
        }
    }

    public HRESULT SetFlags(uint dwFlags)
    {
        return ThisPtr->SetFlags((IShellLinkDataList*)Unsafe.AsPointer(ref this), dwFlags);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkDataList*, void*, HRESULT> AddDataBlock;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkDataList*, uint, void**, HRESULT> CopyDataBlock;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkDataList*, uint, HRESULT> RemoveDataBlock;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkDataList*, uint*, HRESULT> GetFlags;
        internal readonly delegate* unmanaged[Stdcall]<IShellLinkDataList*, uint, HRESULT> SetFlags;
    }
}