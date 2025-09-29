// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using System.Collections.Immutable;

namespace Snap.Hutao.Service;

internal static class Selection
{
    public static TNameValue? Initialize<TNameValue, T>(Lazy<ImmutableArray<TNameValue>> options, T current)
        where TNameValue : NameValue<T>
        where T : struct, IEquatable<T>
    {
        return options.Value.SingleOrDefault(option => option.Value.Equals(current));
    }

    public static TNameValue? Initialize<TNameValue, T>(ImmutableArray<TNameValue> options, T current)
        where TNameValue : NameValue<T>
        where T : notnull
    {
        return options.SingleOrDefault(option => option.Value.Equals(current));
    }

    public static TAny? Initialize<TAny, T>(ImmutableArray<TAny> options, T current, Func<TAny, T> valueSelector, IEqualityComparer<T> comparer)
        where T : notnull
    {
        return options.SingleOrDefault(option => comparer.Equals(valueSelector(option), current));
    }
}