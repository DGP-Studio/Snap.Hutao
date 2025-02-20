// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.System.Com.StructuredStorage;

[SupportedOSPlatform("windows5.0")]
internal static unsafe class IStorage
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x0B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, STGM, uint, uint, nint*, HRESULT> CreateStream;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, void*, STGM, uint, nint*, HRESULT> OpenStream;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, STGM, uint, uint, nint*, HRESULT> CreateStorage;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, nint, STGM, ushort**, uint, nint*, HRESULT> OpenStorage;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, Guid*, ushort**, nint, HRESULT> CopyTo;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, nint, PCWSTR, uint, HRESULT> MoveElementTo;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, HRESULT> Commit;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Revert;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, void*, uint, nint*, HRESULT> EnumElements;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, HRESULT> DestroyElement;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, PCWSTR, HRESULT> RenameElement;
        internal readonly delegate* unmanaged[Stdcall]<nint, PCWSTR, FILETIME*, FILETIME*, FILETIME*, HRESULT> SetElementTimes;
        internal readonly delegate* unmanaged[Stdcall]<nint, Guid*, HRESULT> SetClass;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, uint, HRESULT> SetStateBits;
        internal readonly delegate* unmanaged[Stdcall]<nint, STATSTG*, uint, HRESULT> Stat;
    }
}