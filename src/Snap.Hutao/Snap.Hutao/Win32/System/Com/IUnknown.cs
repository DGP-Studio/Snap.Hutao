// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Win32.System.Com;

internal readonly unsafe struct IUnknown
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly delegate* unmanaged[Stdcall]<IUnknown*, Guid*, void**, HRESULT> QueryInterface;
        internal readonly delegate* unmanaged[Stdcall]<IUnknown*, uint> AddRef;
        internal readonly delegate* unmanaged[Stdcall]<IUnknown*, uint> Release;
    }
}