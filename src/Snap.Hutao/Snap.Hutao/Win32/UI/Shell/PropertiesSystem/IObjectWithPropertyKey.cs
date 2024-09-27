// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.UI.Shell.PropertiesSystem;

[SupportedOSPlatform("windows6.0.6000")]
internal readonly unsafe struct IObjectWithPropertyKey
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xA7, 0xA0, 0x0C, 0xFC, 0x16, 0xC3, 0xD2, 0x4F, 0x90, 0x31, 0x3E, 0x62, 0x8E, 0x6D, 0x4F, 0x23]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IObjectWithPropertyKey*, PROPERTYKEY*, HRESULT> SetPropertyKey;
        internal readonly delegate* unmanaged[Stdcall]<IObjectWithPropertyKey*, PROPERTYKEY*, HRESULT> GetPropertyKey;
    }
}