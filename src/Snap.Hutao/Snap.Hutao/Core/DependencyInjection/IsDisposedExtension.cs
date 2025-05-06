// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.DependencyInjection;

[Obsolete("Use DependencyInjection.DisposeDeferral instead")]
internal static class IsDisposedExtension
{
    [Obsolete("Use DependencyInjection.DisposeDeferral instead")]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void TryThrow(this IIsDisposed isDisposed)
    {
        if (isDisposed.IsDisposed)
        {
            throw new OperationCanceledException("ServiceProvider is disposed");
        }
    }
}