// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Numerics;

namespace Snap.Hutao.Core.Text.Json.Converter;

internal sealed class HexNumberConverter<T> : JsonConverter<T>
    where T : struct, INumberBase<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.String && reader.GetString() is { } str)
        {
            if (str.StartsWith("0x", StringComparison.OrdinalIgnoreCase) &&
                T.TryParse(str.AsSpan(2), System.Globalization.NumberStyles.HexNumber, default, out T hex))
            {
                return hex;
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        writer.WriteStringValue($"0x{value:X}");
    }
}