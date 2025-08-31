// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.IO;

internal readonly struct ValueFile : IEquatable<ValueFile>
{
    private readonly string value;

    private ValueFile(string value)
    {
        this.value = value;
    }

    public bool HasValue { get => value is not null; }

    public static implicit operator string(ValueFile value)
    {
        return value.value;
    }

    public static implicit operator ValueFile(string? value)
    {
        return new(value!);
    }

    [SuppressMessage("", "CA1307")]
    public override int GetHashCode()
    {
        return value.GetHashCode();
    }

    public override string ToString()
    {
        return value;
    }

    public bool Equals(ValueFile other)
    {
        return string.Equals(value, other.value, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueFile other && Equals(other);
    }
}