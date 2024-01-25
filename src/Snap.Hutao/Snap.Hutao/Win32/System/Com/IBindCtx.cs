// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.


// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.System.Com;

internal unsafe struct IBindCtx
{
    internal static Guid IID = new(14u, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

    private Vftbl* thisPtr;

    internal readonly struct Vftbl
    {
        internal readonly IUnknown.Vftbl IUnknownVftbl;
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