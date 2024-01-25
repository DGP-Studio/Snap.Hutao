// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.


// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.System.Com;

internal unsafe struct IRunningObjectTable
{
    internal static Guid IID = new(16u, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

    private Vftbl* thisPtr;

    internal readonly struct Vftbl
    {
        internal readonly IUnknown.Vftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IRunningObjectTable*, ROT_FLAGS, IUnknown*, IMoniker*, uint*, HRESULT> Register;
        internal readonly delegate* unmanaged[Stdcall]<IRunningObjectTable*, uint, HRESULT> Revoke;
        internal readonly delegate* unmanaged[Stdcall]<IRunningObjectTable*, IMoniker*, HRESULT> IsRunning;
        internal readonly delegate* unmanaged[Stdcall]<IRunningObjectTable*, IMoniker*, IUnknown**, HRESULT> GetObject;
        internal readonly delegate* unmanaged[Stdcall]<IRunningObjectTable*, uint, FILETIME*, HRESULT> NoteChangeTime;
        internal readonly delegate* unmanaged[Stdcall]<IRunningObjectTable*, IMoniker*, FILETIME*, HRESULT> GetTimeOfLastChange;
        internal readonly delegate* unmanaged[Stdcall]<IRunningObjectTable*, IEnumMoniker**, HRESULT> EnumRunning;
    }
}