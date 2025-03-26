// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core;

internal static class DisposableMarshal
{
    public static void DisposeAndClear<T>(ref T? disposable)
        where T : class, IDisposable
    {
        if (disposable is not null)
        {
            disposable.Dispose();
            disposable = null;
        }
    }
}