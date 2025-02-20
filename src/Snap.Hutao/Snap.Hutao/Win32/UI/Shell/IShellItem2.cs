// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.System.Com.StructuredStorage;
using Snap.Hutao.Win32.UI.Shell.PropertiesSystem;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32.UI.Shell;

[SupportedOSPlatform("windows6.0.6000")]
internal static unsafe class IShellItem2
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0xD3, 0xB0, 0x9F, 0x7E, 0x9F, 0x91, 0x07, 0x43, 0xAB, 0x2E, 0x9B, 0x18, 0x60, 0x31, 0x0C, 0x93]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IShellItem.Vftbl IShellItemVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, GETPROPERTYSTOREFLAGS, Guid*, void**, HRESULT> GetPropertyStore;
        internal readonly delegate* unmanaged[Stdcall]<nint, GETPROPERTYSTOREFLAGS, nint, Guid*, void**, HRESULT> GetPropertyStoreWithCreateObject;
        internal readonly delegate* unmanaged[Stdcall]<nint, PROPERTYKEY*, uint, GETPROPERTYSTOREFLAGS, Guid*, void**, HRESULT> GetPropertyStoreForKeys;
        internal readonly delegate* unmanaged[Stdcall]<nint, PROPERTYKEY*, Guid*, void**, HRESULT> GetPropertyDescriptionList;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> Update;
        internal readonly delegate* unmanaged[Stdcall]<nint, PROPERTYKEY*, PROPVARIANT*, HRESULT> GetProperty;
        internal readonly delegate* unmanaged[Stdcall]<nint, PROPERTYKEY*, Guid*, HRESULT> GetCLSID;
        internal readonly delegate* unmanaged[Stdcall]<nint, PROPERTYKEY*, FILETIME*, HRESULT> GetFileTime;
        internal readonly delegate* unmanaged[Stdcall]<nint, PROPERTYKEY*, int*, HRESULT> GetInt32;
        internal readonly delegate* unmanaged[Stdcall]<nint, PROPERTYKEY*, PWSTR*, HRESULT> GetString;
        internal readonly delegate* unmanaged[Stdcall]<nint, PROPERTYKEY*, uint*, HRESULT> GetUInt32;
        internal readonly delegate* unmanaged[Stdcall]<nint, PROPERTYKEY*, ulong*, HRESULT> GetUInt64;
        internal readonly delegate* unmanaged[Stdcall]<nint, PROPERTYKEY*, BOOL*, HRESULT> GetBool;
    }
}