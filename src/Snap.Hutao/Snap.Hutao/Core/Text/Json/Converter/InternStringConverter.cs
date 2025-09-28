// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Core.Text.Json.Converter;

internal sealed class InternStringConverter : JsonConverter<string?>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.String => string.Intern(reader.GetString()!),
            _ => throw new JsonException(),
        };
    }

    public override string ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(reader.TokenType is JsonTokenType.PropertyName);
        return string.Intern(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(value!);
    }
}