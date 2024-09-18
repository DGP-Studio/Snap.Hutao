// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.System.Com;

[SupportedOSPlatform("windows5.0")]
internal readonly unsafe struct IBindCtx
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get
        {
            ReadOnlySpan<byte> data = [0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46];
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, IUnknown*, HRESULT> RegisterObjectBound;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, IUnknown*, HRESULT> RevokeObjectBound;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, HRESULT> ReleaseBoundObjects;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, BIND_OPTS*, HRESULT> SetBindOptions;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, BIND_OPTS*, HRESULT> GetBindOptions;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, IRunningObjectTable**, HRESULT> GetRunningObjectTable;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, PWSTR, IUnknown*, HRESULT> RegisterObjectParam;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, PWSTR, IUnknown**, HRESULT> GetObjectParam;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, IEnumString**, HRESULT> EnumObjectParam;
        internal readonly delegate* unmanaged[Stdcall]<IBindCtx*, PWSTR, HRESULT> RevokeObjectParam;
    }
}