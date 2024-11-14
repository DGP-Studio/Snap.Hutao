// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata;

[JsonConverter(typeof(TypeValueCollectionConverter))]
internal sealed class TypeValueCollection<TType, TValue>
    where TType : notnull
{
    private readonly SortedDictionary<TType, TValue> typeValues = [];

    public TypeValueCollection(ImmutableArray<TypeValue<TType, TValue>> entries)
    {
        foreach (ref readonly TypeValue<TType, TValue> entry in entries.AsSpan())
        {
            typeValues.Add(entry.Type, entry.Value);
        }
    }

    internal IReadOnlyDictionary<TType, TValue> TypeValues { get => typeValues; }

    public TValue? GetValueOrDefault(TType type)
    {
        typeValues.TryGetValue(type, out TValue? value);
        return value;
    }
}