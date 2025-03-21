// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Unicode;

namespace Snap.Hutao.Core.Json.Converter;

internal sealed class PooledStringConverter : JsonConverter<string?>
{
    private static readonly ConcurrentDictionary<string, string> Pool = [];
    private static readonly ConcurrentDictionary<string, string>.AlternateLookup<ReadOnlySpan<char>> PoolLookup = Pool.GetAlternateLookup<ReadOnlySpan<char>>();

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType is JsonTokenType.String)
        {
            return Read(reader.ValueSpan);
        }

        throw new JsonException();
    }

    public override string ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(reader.TokenType is JsonTokenType.PropertyName);
        return Read(reader.ValueSpan);
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.AsSpan());
        }
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            throw new JsonException();
        }

        writer.WritePropertyName(value.AsSpan());
    }

    private static string Read(ReadOnlySpan<byte> valueSpan)
    {
        char[] buffer = ArrayPool<char>.Shared.Rent(valueSpan.Length * 2);
        Utf8.ToUtf16(valueSpan, buffer, out _, out int written);
        try
        {
            if (PoolLookup.TryGetValue(buffer.AsSpan(0, written), out string? pooled))
            {
                return pooled;
            }
            else
            {
                string create = new(buffer, 0, written);
                Pool.TryAdd(create, create);
                return create;
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }
}