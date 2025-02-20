// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.System.Com;

[SupportedOSPlatform("windows5.0")]
internal static unsafe class IDataObject
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x0E, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, FORMATETC*, STGMEDIUM*, HRESULT> GetData;
        internal readonly delegate* unmanaged[Stdcall]<nint, FORMATETC*, STGMEDIUM*, HRESULT> GetDataHere;
        internal readonly delegate* unmanaged[Stdcall]<nint, FORMATETC*, HRESULT> QueryGetData;
        internal readonly delegate* unmanaged[Stdcall]<nint, FORMATETC*, FORMATETC*, HRESULT> GetCanonicalFormatEtc;
        internal readonly delegate* unmanaged[Stdcall]<nint, FORMATETC*, STGMEDIUM*, BOOL, HRESULT> SetData;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, nint*, HRESULT> EnumFormatEtc;
        internal readonly delegate* unmanaged[Stdcall]<nint, FORMATETC*, uint, nint, uint*, HRESULT> DAdvise;
        internal readonly delegate* unmanaged[Stdcall]<nint, uint, HRESULT> DUnadvise;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, HRESULT> EnumDAdvise;
    }
}