// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.UI.Shell.Common;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows6.0.6000")]
internal static unsafe class IEnumFullIDList
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x42, 0x15, 0x19, 0xD0, 0x54, 0x79, 0x08, 0x49, 0xBC, 0x06, 0xB2, 0x36, 0x0B, 0xBE, 0x45, 0xBA]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, ITEMIDLIST**, uint*, HRESULT> Next;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, HRESULT> Skip;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> Reset;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, HRESULT> Clone;
    }
}