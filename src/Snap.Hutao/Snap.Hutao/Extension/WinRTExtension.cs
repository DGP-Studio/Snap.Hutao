// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using WinRT;

namespace Snap.Hutao.Extension;

internal static class WinRTExtension
{
    public static bool IsDisposed(this IWinRTObject obj)
    {
        return Volatile.Read(ref GetPrivateDisposedFlags(obj.NativeObject)) is not 0;
    }

    // private int _disposedFlags
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_disposedFlags")]
    private static extern ref int GetPrivateDisposedFlags(IObjectReference objRef);
}