// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive.Converter;

namespace Snap.Hutao.Model.Primitive;

/// <summary>
/// 7位 装备属性Id
/// </summary>
[JsonConverter(typeof(IdentityConverter<ExtendedEquipAffixId>))]
public readonly struct ExtendedEquipAffixId : IEquatable<ExtendedEquipAffixId>
{
    /// <summary>
    /// 值
    /// </summary>
    public readonly int Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedEquipAffixId"/> struct.
    /// </summary>
    /// <param name="value">value</param>
    public ExtendedEquipAffixId(int value)
    {
        Value = value;
    }

    public static implicit operator int(ExtendedEquipAffixId value)
    {
        return value.Value;
    }

    public static implicit operator ExtendedEquipAffixId(int value)
    {
        return new(value);
    }

    public static bool operator ==(ExtendedEquipAffixId left, ExtendedEquipAffixId right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(ExtendedEquipAffixId left, ExtendedEquipAffixId right)
    {
        return !(left == right);
    }

    /// <inheritdoc/>
    public bool Equals(ExtendedEquipAffixId other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is ExtendedEquipAffixId other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}