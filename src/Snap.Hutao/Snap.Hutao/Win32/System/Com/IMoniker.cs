// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.System.Com;

internal unsafe struct IMoniker
{
    internal static Guid IID = new(15u, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

    private Vftbl* thisPtr;

    internal readonly struct Vftbl
    {
        internal readonly IPersistStream.Vftbl IPersistStreamVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IBindCtx*, IMoniker*, Guid*, void**, HRESULT> BindToObject;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IBindCtx*, IMoniker*, Guid*, void**, HRESULT> BindToStorage;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IBindCtx*, uint, IMoniker**, IMoniker**, HRESULT> Reduce;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IMoniker*, BOOL, IMoniker**, HRESULT> ComposeWith;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, BOOL, IEnumMoniker**, HRESULT> Enum;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IMoniker*, HRESULT> IsEqual;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, uint*, HRESULT> Hash;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IBindCtx*, IMoniker*, IMoniker*, HRESULT> IsRunning;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IBindCtx*, IMoniker*, FILETIME*, HRESULT> GetTimeOfLastChange;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IMoniker**, HRESULT> Inverse;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IMoniker*, IMoniker**, HRESULT> CommonPrefixWith;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IMoniker*, IMoniker**, HRESULT> RelativePathTo;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IBindCtx*, IMoniker*, PWSTR*, HRESULT> GetDisplayName;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, IBindCtx*, IMoniker*, PWSTR, uint*, IMoniker**, HRESULT> ParseDisplayName;
        internal readonly delegate* unmanaged[Stdcall]<IMoniker*, uint*, HRESULT> IsSystemMoniker;
    }
}
