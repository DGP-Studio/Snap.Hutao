// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive.Converter;

namespace Snap.Hutao.Model.Primitive;

/// <summary>
/// 6位 圣遗物副词条Id
/// </summary>
[HighQuality]
[JsonConverter(typeof(IdentityConverter<ReliquaryAffixId>))]
internal readonly struct ReliquaryAffixId : IEquatable<ReliquaryAffixId>
{
    /// <summary>
    /// 值
    /// </summary>
    public readonly int Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReliquaryAffixId"/> struct.
    /// </summary>
    /// <param name="value">value</param>
    public ReliquaryAffixId(int value)
    {
        Value = value;
    }

    public static implicit operator int(ReliquaryAffixId value)
    {
        return value.Value;
    }

    public static implicit operator ReliquaryAffixId(int value)
    {
        return new(value);
    }

    public static bool operator ==(ReliquaryAffixId left, ReliquaryAffixId right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(ReliquaryAffixId left, ReliquaryAffixId right)
    {
        return !(left == right);
    }

    /// <inheritdoc/>
    public bool Equals(ReliquaryAffixId other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is ReliquaryAffixId other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}