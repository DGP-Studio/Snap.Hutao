// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using WinRT;

namespace Snap.Hutao.Extension;

// ReSharper disable once InconsistentNaming
internal static class WinRTExtension
{
    public static bool IsDisposed(this IWinRTObject? obj)
    {
        return obj?.NativeObject is null || obj.NativeObject.IsDisposed();
    }

    public static bool IsDisposed(this IObjectReference objRef)
    {
        return Volatile.Read(ref GetPrivateDisposedFlags(objRef)) is not 0;
    }

    // private const int NOT_DISPOSED = 0;
    // private const int DISPOSE_PENDING = 1;
    // private const int DISPOSE_COMPLETED = 2;
    // private int _disposedFlags
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_disposedFlags")]
    private static extern ref int GetPrivateDisposedFlags(IObjectReference objRef);
}