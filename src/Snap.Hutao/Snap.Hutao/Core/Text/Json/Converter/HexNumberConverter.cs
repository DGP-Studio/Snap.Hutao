// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;
using System.Numerics;

namespace Snap.Hutao.Core.Text.Json.Converter;

internal sealed class HexNumberConverter<T> : JsonConverter<T>
    where T : struct, INumberBase<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.String)
        {
            if ((reader.ValueSpan.StartsWith("0x"u8) || reader.ValueSpan.StartsWith("0X"u8)) &&
                T.TryParse(reader.ValueSpan[2..], NumberStyles.HexNumber, default, out T hex))
            {
                return hex;
            }
        }
        else if (reader.TokenType is JsonTokenType.Number && reader.TryGetInt64(out long value))
        {
            return T.Parse(reader.ValueSpan, CultureInfo.CurrentCulture);
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        writer.WriteStringValue($"0x{value:X}");
    }
}