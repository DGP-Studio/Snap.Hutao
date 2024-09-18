// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.System.Com;

[SupportedOSPlatform("windows5.0")]
internal readonly unsafe struct IEnumMoniker
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IEnumMoniker*, uint, IMoniker**, uint*, HRESULT> Next;
        internal readonly delegate* unmanaged[Stdcall]<IEnumMoniker*, uint, HRESULT> Skip;
        internal readonly delegate* unmanaged[Stdcall]<IEnumMoniker*, HRESULT> Reset;
        internal readonly delegate* unmanaged[Stdcall]<IEnumMoniker*, IEnumMoniker**, HRESULT> Clone;
    }
}
