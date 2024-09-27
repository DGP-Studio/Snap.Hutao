// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.System.Com;

[SupportedOSPlatform("windows5.0")]
internal readonly unsafe struct IPersistStream
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x09, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IPersist.Vftbl IPersistVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IPersistStream*, HRESULT> IsDirty;
        internal readonly delegate* unmanaged[Stdcall]<IPersistStream*, IStream*, HRESULT> Load;
        internal readonly delegate* unmanaged[Stdcall]<IPersistStream*, IStream*, BOOL, HRESULT> Save;
        internal readonly delegate* unmanaged[Stdcall]<IPersistStream*, ulong*, HRESULT> GetSizeMax;
    }
}
