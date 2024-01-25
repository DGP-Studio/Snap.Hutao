// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.


// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.System.Com;

internal unsafe struct IPersistStream
{
    internal static Guid IID = new(265u, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

    private Vftbl* thisPtr;

    internal readonly struct Vftbl
    {
        internal readonly IPersist.Vftbl IPersistVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IPersistStream*, HRESULT> IsDirty;
        internal readonly delegate* unmanaged[Stdcall]<IPersistStream*, IStream*, HRESULT> Load;
        internal readonly delegate* unmanaged[Stdcall]<IPersistStream*, IStream*, BOOL, HRESULT> Save;
        internal readonly delegate* unmanaged[Stdcall]<IPersistStream*, ulong*, HRESULT> GetSizeMax;
    }
}
