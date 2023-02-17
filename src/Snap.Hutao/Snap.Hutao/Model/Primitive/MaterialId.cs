// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive.Converter;

namespace Snap.Hutao.Model.Primitive;

/// <summary>
/// 3-6位 材料Id
/// </summary>
[HighQuality]
[JsonConverter(typeof(IdentityConverter<MaterialId>))]
internal readonly struct MaterialId : IEquatable<MaterialId>, IComparable<MaterialId>
{
    /// <summary>
    /// 值
    /// </summary>
    public readonly int Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialId"/> struct.
    /// </summary>
    /// <param name="value">value</param>
    public MaterialId(int value)
    {
        Value = value;
    }

    public static implicit operator int(MaterialId value)
    {
        return value.Value;
    }

    public static implicit operator MaterialId(int value)
    {
        return new(value);
    }

    public static bool operator ==(MaterialId left, MaterialId right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(MaterialId left, MaterialId right)
    {
        return !(left == right);
    }

    /// <inheritdoc/>
    public int CompareTo(MaterialId other)
    {
        return Value.CompareTo(other.Value);
    }

    /// <inheritdoc/>
    public bool Equals(MaterialId other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is MaterialId other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}