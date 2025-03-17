// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Primitive.Converter;

internal sealed unsafe class IdentityConverter<TWrapper> : JsonConverter<TWrapper>
    where TWrapper : unmanaged
{
    public override TWrapper Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        uint value = JsonSerializer.Deserialize<uint>(ref reader, options);
        return *(TWrapper*)&value;
    }

    public override TWrapper ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }

        string? value = reader.GetString();
        _ = uint.TryParse(value, out uint result);
        return *(TWrapper*)&result;
    }

    public override void Write(Utf8JsonWriter writer, TWrapper value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, *(uint*)&value, options);
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, TWrapper value, JsonSerializerOptions options)
    {
        writer.WritePropertyName($"{*(uint*)&value}");
    }
}