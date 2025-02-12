// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.Collection;

#pragma warning disable SA1402
internal static class SpanAccess
{
    public static ISpanAccess<T> Create<T>(T[]? array)
    {
        return new ArraySpanAccess<T>(array);
    }

    public static IReadOnlySpanAccess<T> CreateReadOnly<T>(T[]? array)
    {
        return new ArraySpanAccess<T>(array);
    }

    public static IReadOnlySpanAccess<T> CreateReadOnly<T>(ImmutableArray<T> immutableArray)
    {
        return new ImmutableArraySpanAccess<T>(immutableArray);
    }

    public static ISpanAccess<T> Create<T>(List<T> list)
    {
        return new ListSpanAccess<T>(list);
    }

    public static IReadOnlySpanAccess<T> CreateReadOnly<T>(List<T> list)
    {
        return new ListSpanAccess<T>(list);
    }
}

file class ArraySpanAccess<T> : ISpanAccess<T>
{
    private readonly T[]? array;

    public ArraySpanAccess(T[]? array)
    {
        this.array = array;
    }

    public ReadOnlySpan<T> ReadOnlySpan { get => array.AsSpan(); }

    public Span<T> Span { get => array.AsSpan(); }
}

file class ImmutableArraySpanAccess<T> : IReadOnlySpanAccess<T>
{
    private readonly ImmutableArray<T> immutableArray;

    public ImmutableArraySpanAccess(ImmutableArray<T> immutableArray)
    {
        this.immutableArray = immutableArray;
    }

    public ReadOnlySpan<T> ReadOnlySpan { get => immutableArray.AsSpan(); }
}

file class ListSpanAccess<T> : ISpanAccess<T>
{
    private readonly List<T>? list;

    public ListSpanAccess(List<T>? list)
    {
        this.list = list;
    }

    public ReadOnlySpan<T> ReadOnlySpan { get => CollectionsMarshal.AsSpan(list); }

    public Span<T> Span { get => CollectionsMarshal.AsSpan(list); }
}