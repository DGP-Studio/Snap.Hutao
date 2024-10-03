// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.System.Com;

[SupportedOSPlatform("windows5.0")]
internal static unsafe class IBindCtx
{
    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> RegisterObjectBound;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint, HRESULT> RevokeObjectBound;
        internal readonly delegate* unmanaged[Stdcall]<nint, HRESULT> ReleaseBoundObjects;
        internal readonly delegate* unmanaged[Stdcall]<nint, BIND_OPTS*, HRESULT> SetBindOptions;
        internal readonly delegate* unmanaged[Stdcall]<nint, BIND_OPTS*, HRESULT> GetBindOptions;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, HRESULT> GetRunningObjectTable;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, nint, HRESULT> RegisterObjectParam;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, nint*, HRESULT> GetObjectParam;
        internal readonly delegate* unmanaged[Stdcall]<nint, nint*, HRESULT> EnumObjectParam;
        internal readonly delegate* unmanaged[Stdcall]<nint, PWSTR, HRESULT> RevokeObjectParam;
    }
}