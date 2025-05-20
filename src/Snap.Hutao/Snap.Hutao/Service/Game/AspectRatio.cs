// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Concurrent;

namespace Snap.Hutao.Service.Game;

[JsonConverter(typeof(AspectRatioConverter))]
internal sealed class AspectRatio : IEquatable<AspectRatio>
{
    public AspectRatio(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public double Width { get; }

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

[SuppressMessage("", "SA1402")]
internal sealed class AspectRatioConverter : JsonConverter<AspectRatio>
{
    public const string WidthPropertyName = "width";
    public const string HeightPropertyName = "height";

    // AspectRatio is marshaled to WinRT as nint, so we should cache instances and reuse them.
    private static readonly ConcurrentDictionary<(double Width, double Height), AspectRatio> AspectRatioPool = [];

    public override AspectRatio Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        double? width = null;
        double? height = null;

        while (reader.Read())
        {
            if (reader.TokenType is JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType is not JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string propertyName = reader.GetString()!;
            reader.Read();

            switch (propertyName)
            {
                case WidthPropertyName:
                    width = reader.GetDouble();
                    break;
                case HeightPropertyName:
                    height = reader.GetDouble();
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        if (width is null || height is null)
        {
            throw new JsonException("Missing required properties for AspectRatio");
        }

        if (AspectRatioPool.TryGetValue((width.Value, height.Value), out AspectRatio? cached))
        {
            return cached;
        }

        AspectRatio value = new(width.Value, height.Value);
        AspectRatioPool.TryAdd((width.Value, height.Value), value);
        return value;
    }

    public override void Write(Utf8JsonWriter writer, AspectRatio value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber(WidthPropertyName, value.Width);
        writer.WriteNumber(HeightPropertyName, value.Height);
        writer.WriteEndObject();
    }
}