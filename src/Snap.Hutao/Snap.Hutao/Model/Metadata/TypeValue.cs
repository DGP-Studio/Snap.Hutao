// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata;

internal class TypeValue<TType, TValue>
{
    public TypeValue(TType type, TValue value)
    {
        Type = type;
        Value = value;
    }

    public TType Type { get; }

    public TValue Value { get; }

    public override bool Equals(object? obj)
    {
        return obj is TypeValue<TType, TValue> value
            && EqualityComparer<TType>.Default.Equals(Type, value.Type)
            && EqualityComparer<TValue>.Default.Equals(Value, value.Value);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Value);
    }
}