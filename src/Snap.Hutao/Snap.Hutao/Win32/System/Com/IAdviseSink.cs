// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.System.Com;

[SupportedOSPlatform("windows5.0")]
internal static unsafe class IAdviseSink
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x0F, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x0000000046]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, FORMATETC*, STGMEDIUM*, void> OnDataChange;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, int, void> OnViewChange;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, void> OnRename;
        internal readonly delegate* unmanaged[Stdcall]<nint, void> OnSave;
        internal readonly delegate* unmanaged[Stdcall]<nint, void> OnClose;
    }
}