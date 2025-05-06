// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.DependencyInjection;

internal static class IsDisposedExtension
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void TryThrow(this IIsDisposed isDisposed)
    {
        if (isDisposed.IsDisposed)
        {
            throw new OperationCanceledException("ServiceProvider is disposed");
        }
    }
}