// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Concurrent;

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

internal sealed class AspectRatioConverter : JsonConverter<AspectRatio>
{
    // AspectRatio is marshaled to WinRT as nint, so we should cache instances and reuse them.
    private static readonly ConcurrentDictionary<AspectRatio, AspectRatio> AspectRatioPool = [];

    public override AspectRatio Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        AspectRatio? value = JsonSerializer.Deserialize<AspectRatio>(ref reader, options);
        if (value is null)
        {
            throw new JsonException();
        }

        if (AspectRatioPool.TryGetValue(value, out AspectRatio? cached))
        {
            return cached;
        }

        AspectRatioPool.TryAdd(value, value);
        return value;
    }

    public override void Write(Utf8JsonWriter writer, AspectRatio value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}