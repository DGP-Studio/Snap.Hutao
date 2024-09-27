// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com.StructuredStorage;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.UI.Shell.PropertiesSystem;

[SupportedOSPlatform("windows6.0.6000")]
internal readonly unsafe struct IPropertyChange
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x8A, 0xBC, 0x17, 0xF9, 0xBA, 0x1B, 0x78, 0x44, 0xA2, 0x45, 0x1B, 0xDE, 0x03, 0xEB, 0x94, 0x31]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IObjectWithPropertyKey.Vftbl IObjectWithPropertyKeyVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IPropertyChange*, PROPVARIANT*, PROPVARIANT*, HRESULT> ApplyToPropVariant;
    }
}
