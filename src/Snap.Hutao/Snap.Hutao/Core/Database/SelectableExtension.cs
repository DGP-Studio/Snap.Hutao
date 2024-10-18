// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database.Abstraction;
using Snap.Hutao.Core.ExceptionService;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Database;

internal static class SelectableExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TSource? SelectedOrDefault<TSource>(this IEnumerable<TSource> source)
        where TSource : ISelectable
    {
        return source.SingleOrDefault(IsSelected);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TSource? SelectedOrFirstOrDefault<TSource>(this IEnumerable<TSource> source)
        where TSource : ISelectable
    {
        using (IEnumerator<TSource> e = source.GetEnumerator())
        {
            if (!e.MoveNext())
            {
                return default;
            }

            TSource first = e.Current;

            do
            {
                TSource result = e.Current;
                if (!result.IsSelected)
                {
                    continue;
                }

                while (e.MoveNext())
                {
                    if (e.Current.IsSelected)
                    {
                        HutaoException.InvalidOperation("Multiple item is selected.");
                    }
                }

                return result;
            }
            while (e.MoveNext());

            return first;
        }
    }

    private static bool IsSelected<TSelectable>(TSelectable item)
        where TSelectable : ISelectable
    {
        return item.IsSelected;
    }
}