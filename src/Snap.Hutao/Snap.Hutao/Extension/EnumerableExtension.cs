// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Extension;

internal static partial class EnumerableExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
        return new ObservableCollection<T>(source);
    }

    public static string ToString<T>(this IEnumerable<T> collection, char separator)
    {
        return string.Join(separator, collection);
    }
}