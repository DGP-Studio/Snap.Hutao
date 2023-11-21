// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using WinRT;

namespace Snap.Hutao.Extension;

internal static class WinRTExtension
{
    public static bool IsDisposed(this IWinRTObject obj)
    {
        return GetDisposed(obj.NativeObject);
    }

    // protected bool disposed;
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name ="disposed")]
    private static extern ref bool GetDisposed(IObjectReference objRef);
}