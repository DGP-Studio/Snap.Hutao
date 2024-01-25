// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.


// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.System.Com;

internal unsafe struct IStream
{
    internal static Guid IID = new(12u, 0, 0, 192, 0, 0, 0, 0, 0, 0, 70);

    private Vftbl* thisPtr;

    internal readonly struct Vftbl
    {
        internal readonly ISequentialStream.Vftbl ISequentialStreamVftbl;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, long, STREAM_SEEK, ulong*, HRESULT> Seek;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, ulong, HRESULT> SetSize;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, IStream*, ulong, ulong*, ulong*, HRESULT> CopyTo;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, uint, HRESULT> Commit;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, HRESULT> Revert;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, ulong, ulong, uint, HRESULT> LockRegion;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, ulong, ulong, uint, HRESULT> UnlockRegion;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, STATSTG*, uint, HRESULT> Stat;
        internal readonly delegate* unmanaged[Stdcall]<IStream*, IStream**, HRESULT> Clone;
    }
}