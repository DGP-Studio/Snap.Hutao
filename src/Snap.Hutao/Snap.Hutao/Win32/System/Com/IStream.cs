// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.System.Com;

[SupportedOSPlatform("windows5.0")]
internal readonly unsafe struct IStream
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46]);
    }

    internal readonly struct Vftbl
    {
        internal readonly ISequentialStream.Vftbl ISequentialStreamVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, long, STREAM_SEEK, ulong*, HRESULT> Seek;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, ulong, HRESULT> SetSize;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, IStream*, ulong, ulong*, ulong*, HRESULT> CopyTo;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, uint, HRESULT> Commit;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, HRESULT> Revert;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, ulong, ulong, uint, HRESULT> LockRegion;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, ulong, ulong, uint, HRESULT> UnlockRegion;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, STATSTG*, uint, HRESULT> Stat;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, IStream**, HRESULT> Clone;
    }
}