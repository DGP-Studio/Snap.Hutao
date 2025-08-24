// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.UI.Xaml.Data;

internal static class AdvancedCollectionViewExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAdvancedCollectionView<T> AsAdvancedCollectionView<T>(this IEnumerable<T> source)
        where T : class, IPropertyValuesProvider
    {
        return source switch
        {
            IAdvancedCollectionView<T> advancedCollectionView => advancedCollectionView,
            IList<T> list => new AdvancedCollectionView<T>(list),
            _ => new AdvancedCollectionView<T>([.. source]),
        };
    }
}