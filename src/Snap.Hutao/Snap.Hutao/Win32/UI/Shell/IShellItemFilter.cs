// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows6.0.6000")]
internal readonly unsafe struct IShellItemFilter
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x75, 0xB4, 0x59, 0x26, 0xB8, 0xEE, 0xB7, 0x48, 0x8F, 0x07, 0xB3, 0x78, 0x81, 0x0F, 0x48, 0xCF];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknown.Vftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IShellItemFilter*, IShellItem*, HRESULT> IncludeItem;
        internal readonly delegate* unmanaged[Stdcall]<IShellItemFilter*, IShellItem*, uint*, HRESULT> GetEnumFlagsForItem;
    }
}