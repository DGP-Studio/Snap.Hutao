// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive.Converter;

namespace Snap.Hutao.Model.Primitive;

/// <summary>
/// 角色Id
/// </summary>
[JsonConverter(typeof(AvatarIdConverter))]
public readonly struct AvatarId : IEquatable<AvatarId>
{
    /// <summary>
    /// 值
    /// </summary>
    public readonly int Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="AvatarId"/> struct.
    /// </summary>
    /// <param name="value">value</param>
    public AvatarId(int value)
    {
        Value = value;
    }

    public static implicit operator int(AvatarId value)
    {
        return value.Value;
    }

    public static implicit operator AvatarId(int value)
    {
        return new(value);
    }

    public static bool operator ==(AvatarId left, AvatarId right)
    {
        return left.Value == right.Value;
    }

    public static bool operator !=(AvatarId left, AvatarId right)
    {
        return !(left == right);
    }

    /// <inheritdoc/>
    public bool Equals(AvatarId other)
    {
        return Value == other.Value;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is AvatarId other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}