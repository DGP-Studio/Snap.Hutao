// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive.Converter;

namespace Snap.Hutao.Model.Primitive;

/// <summary>
/// 5位 圣遗物主属性Id
/// </summary>
[JsonConverter(typeof(IdentityConverter<ReliquaryMainAffixId>))]
public readonly struct ReliquaryMainAffixId : IEquatable<ReliquaryMainAffixId>
{
    /// <summary>
    /// 值
    /// </summary>
    public readonly int Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReliquaryMainAffixId"/> struct.
    /// </summary>
    /// <param name="value">value</param>
    public ReliquaryMainAffixId(int value)
    {
        Value = value;
    }

    public static implicit operator int(ReliquaryMainAffixId value)
    {
        return value.Value;
    }

    public static implicit operator ReliquaryMainAffixId(int value)
    {
        return new(value);
    }

    public static bool operator ==(ReliquaryMainAffixId left, ReliquaryMainAffixId right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(ReliquaryMainAffixId left, ReliquaryMainAffixId right)
    {
        return !(left == right);
    }

    /// <inheritdoc/>
    public bool Equals(ReliquaryMainAffixId other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is ReliquaryMainAffixId other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}