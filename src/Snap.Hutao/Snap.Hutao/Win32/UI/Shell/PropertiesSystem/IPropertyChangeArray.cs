// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.UI.Shell.PropertiesSystem;

[SupportedOSPlatform("windows6.0.6000")]
internal static unsafe class IPropertyChangeArray
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xAD, 0x5C, 0x0F, 0x38, 0x5E, 0x1B, 0xF2, 0x42, 0x80, 0x5D, 0x63, 0x7F, 0xD3, 0x92, 0xD3, 0x1E]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint*, HRESULT> GetCount;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, Guid*, void**, HRESULT> GetAt;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint, HRESULT> InsertAt;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> Append;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> AppendOrReplace;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, HRESULT> RemoveAt;
        internal readonly delegate* unmanaged[Stdcall]<nint, PROPERTYKEY*, HRESULT> IsKeyInArray;
    }
}