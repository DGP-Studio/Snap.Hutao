// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.System.Com;

[SupportedOSPlatform("windows5.0")]
internal static unsafe class IEnumString
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, PWSTR*, uint*, HRESULT> Next;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, HRESULT> Skip;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Reset;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, HRESULT> Clone;
    }
}