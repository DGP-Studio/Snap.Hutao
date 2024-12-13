// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Model;

internal static class ImmutableCollectionsNameValue
{
    public static ImmutableArray<NameValue<TEnum>> FromEnum<TEnum>()
        where TEnum : struct, Enum
    {
        return ImmutableCollectionsMarshal.AsImmutableArray(Enum.GetValues<TEnum>()).SelectAsArray(DefaultCreateNameValue);
    }

    public static ImmutableArray<NameValue<TEnum>> FromEnum<TEnum>(Func<TEnum, bool> predicate)
        where TEnum : struct, Enum
    {
        return From(Enum.GetValues<TEnum>(), predicate);
    }

    public static ImmutableArray<NameValue<TEnum>> FromEnum<TEnum>(Func<TEnum, string> nameSelector)
        where TEnum : struct, Enum
    {
        return From(Enum.GetValues<TEnum>(), nameSelector);
    }

    public static ImmutableArray<NameValue<TSource>> From<TSource>(IEnumerable<TSource> sources)
    {
        return [.. sources.Select(DefaultCreateNameValue)];
    }

    public static ImmutableArray<NameValue<TSource>> From<TSource>(IEnumerable<TSource> sources, Func<TSource, bool> predicate)
    {
        return [.. sources.Where(predicate).Select(DefaultCreateNameValue)];
    }

    public static ImmutableArray<NameValue<TSource>> From<TSource>(IEnumerable<TSource> sources, Func<TSource, string> nameSelector)
    {
        return [.. sources.Select(x => new NameValue<TSource>(nameSelector(x), x))];
    }

    private static NameValue<TEnum> DefaultCreateNameValue<TEnum>(TEnum value)
    {
        string? name = value?.ToString();
        ArgumentNullException.ThrowIfNull(name);
        return new(name, value);
    }
}