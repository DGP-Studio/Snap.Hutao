// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.UI.Xaml.Data;

internal static class AdvancedCollectionViewCurrentChanged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Attach<T>(IAdvancedCollectionView<T>? acv, EventHandler<object> handler)
        where T : class
    {
        if (acv is null)
        {
            return;
        }

        acv.CurrentChanged += handler;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Detach<T>(IAdvancedCollectionView<T>? acv, EventHandler<object> handler)
        where T : class
    {
        if (acv is null)
        {
            return;
        }

        acv.CurrentChanged -= handler;
    }
}