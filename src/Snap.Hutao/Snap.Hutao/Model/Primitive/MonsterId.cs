// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive.Converter;

namespace Snap.Hutao.Model.Primitive;

/// <summary>
/// 8位 怪物Id
/// </summary>
[HighQuality]
[JsonConverter(typeof(IdentityConverter<MonsterId>))]
internal readonly struct MonsterId : IEquatable<MonsterId>
{
    /// <summary>
    /// 值
    /// </summary>
    public readonly int Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonsterId"/> struct.
    /// </summary>
    /// <param name="value">value</param>
    public MonsterId(int value)
    {
        Value = value;
    }

    public static implicit operator int(MonsterId value)
    {
        return value.Value;
    }

    public static implicit operator MonsterId(int value)
    {
        return new(value);
    }

    public static bool operator ==(MonsterId left, MonsterId right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(MonsterId left, MonsterId right)
    {
        return !(left == right);
    }

    /// <inheritdoc/>
    public bool Equals(MonsterId other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is MonsterId other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Value.ToString();
    }
}