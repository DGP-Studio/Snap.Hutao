// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.UI.Xaml.Data;

internal static class AdvancedCollectionViewExtension
{
    public static void MoveCurrentToFirstOrDefault<T>(this IAdvancedCollectionView<T> view)
        where T : class
    {
        if (!view.MoveCurrentToFirst())
        {
            view.MoveCurrentTo(default);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAdvancedCollectionView<T> AsAdvancedCollectionView<T>(this IList<T> source)
        where T : class, IAdvancedCollectionViewItem
    {
        return new AdvancedCollectionView<T>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAdvancedCollectionView<T> AsAdvancedCollectionView<T>(this IEnumerable<T> source)
        where T : class, IAdvancedCollectionViewItem
    {
        if (source is IList<T> list)
        {
            return new AdvancedCollectionView<T>(list);
        }

        return new AdvancedCollectionView<T>([.. source]);
    }
}