// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata;

[JsonConverter(typeof(TypeValueCollectionConverter))]
internal sealed partial class TypeValueCollection<TType, TValue>
    where TType : notnull
{
    private readonly SortedDictionary<TType, TValue> inner = [];

    public TypeValueCollection(ImmutableArray<TypeValue<TType, TValue>> entries)
    {
        foreach (ref readonly TypeValue<TType, TValue> entry in entries.AsSpan())
        {
            inner.Add(entry.Type, entry.Value);
        }
    }

    internal SortedDictionary<TType, TValue> Inner { get => inner; }

    public TValue? GetValueOrDefault(TType type)
    {
        inner.TryGetValue(type, out TValue? value);
        return value;
    }
}