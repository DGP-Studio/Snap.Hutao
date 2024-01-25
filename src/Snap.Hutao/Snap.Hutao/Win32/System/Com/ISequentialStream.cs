// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.


// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;

namespace Snap.Hutao.Win32.System.Com;

internal unsafe struct ISequentialStream
{
    internal static Guid IID = new(208878128u, 10780, 4558, 173, 229, 0, 170, 0, 68, 119, 61);

    private Vftbl* thisPtr;

    internal readonly struct Vftbl
    {
        internal readonly IUnknown.Vftbl IUnknownVftbl;
        internal readonly delegate* unmanaged[Stdcall]<ISequentialStream*, void*, uint, uint*, HRESULT> Read;
        internal readonly delegate* unmanaged[Stdcall]<ISequentialStream*, void*, uint, uint*, HRESULT> Write;
    }
}