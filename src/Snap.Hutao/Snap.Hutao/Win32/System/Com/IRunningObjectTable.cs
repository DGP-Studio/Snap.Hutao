// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WinRT.Interop;

namespace Snap.Hutao.Win32.System.Com;

[SupportedOSPlatform("windows5.0")]
internal readonly unsafe struct IRunningObjectTable
{
    public readonly Vftbl* ThisPtr;

    internal static ref readonly Guid IID
    {
        get => ref MemoryMarshal.AsRef<Guid>([0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46]);
    }

    internal readonly struct Vftbl
    {
        internal readonly IUnknownVftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IRunningObjectTable*, ROT_FLAGS, IUnknown*, IMoniker*, uint*, HRESULT> Register;
        internal readonly delegate* unmanaged[Stdcall]<IRunningObjectTable*, uint, HRESULT> Revoke;
        internal readonly delegate* unmanaged[Stdcall]<IRunningObjectTable*, IMoniker*, HRESULT> IsRunning;
        internal readonly delegate* unmanaged[Stdcall]<IRunningObjectTable*, IMoniker*, IUnknown**, HRESULT> GetObject;
        internal readonly delegate* unmanaged[Stdcall]<IRunningObjectTable*, uint, FILETIME*, HRESULT> NoteChangeTime;
        internal readonly delegate* unmanaged[Stdcall]<IRunningObjectTable*, IMoniker*, FILETIME*, HRESULT> GetTimeOfLastChange;
        internal readonly delegate* unmanaged[Stdcall]<IRunningObjectTable*, IEnumMoniker**, HRESULT> EnumRunning;
    }
}