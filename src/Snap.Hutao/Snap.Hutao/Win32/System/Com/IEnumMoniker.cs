// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.


// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.System.Com;

internal unsafe struct IEnumMoniker
{
    internal static Guid IID = new(258u, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

    private Vftbl* thisPtr;

    internal readonly struct Vftbl
    {
        internal readonly IUnknown.Vftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IEnumMoniker*, uint, IMoniker**, uint*, HRESULT> Next;
        internal readonly delegate* unmanaged[Stdcall]<IEnumMoniker*, uint, HRESULT> Skip;
        internal readonly delegate* unmanaged[Stdcall]<IEnumMoniker*, HRESULT> Reset;
        internal readonly delegate* unmanaged[Stdcall]<IEnumMoniker*, IEnumMoniker**, HRESULT> Clone;
    }
}
