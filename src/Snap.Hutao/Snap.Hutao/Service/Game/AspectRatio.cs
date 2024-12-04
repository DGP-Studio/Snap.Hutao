// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Game;

internal sealed class AspectRatio : IEquatable<AspectRatio>
{
    public AspectRatio(double width, double height)
    {
        Width = width;
        Height = height;
    }

    [JsonPropertyName("width")]
    public double Width { get; }

    [JsonPropertyName("height")]
    public double Height { get; }

    public override string ToString()
    {
        return $"{Width}:{Height}";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public bool Equals(AspectRatio? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Width.Equals(other.Width) && Height.Equals(other.Height);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is AspectRatio other && Equals(other));
    }
}