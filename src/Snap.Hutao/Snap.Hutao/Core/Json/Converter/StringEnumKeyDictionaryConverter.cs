// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Json.Converter;

/// <summary>
/// Json枚举键字典转换器
/// </summary>
[HighQuality]
internal sealed class StringEnumKeyDictionaryConverter : JsonConverterFactory
{
    private readonly Type converterType = typeof(StringEnumDictionaryConverterInner<,>);

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        if (typeToConvert.GetGenericTypeDefinition() != typeof(Dictionary<,>))
        {
            return false;
        }

        return typeToConvert.GetGenericArguments()[0].IsEnum;
    }

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
    {
        Type innerConverterType = converterType.MakeGenericType(type.GetGenericArguments());
        JsonConverter converter = (JsonConverter)Activator.CreateInstance(innerConverterType)!;
        return converter;
    }

    private class StringEnumDictionaryConverterInner<TKey, TValue> : JsonConverter<IDictionary<TKey, TValue>>
        where TKey : struct, Enum
    {
        private readonly Type keyType;

        public StringEnumDictionaryConverterInner()
        {
            // Cache the key and value types.
            keyType = typeof(TKey);
        }

        public override IDictionary<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            Dictionary<TKey, TValue> dictionary = new();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return dictionary;
                }

                // Get the key.
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                string? propertyName = reader.GetString();

                if (!Enum.TryParse(propertyName, false, out TKey key) && !Enum.TryParse(propertyName, true, out key))
                {
                    throw new JsonException($"Unable to convert \"{propertyName}\" to Enum \"{keyType}\".");
                }

                // Get the value.
                TValue value = JsonSerializer.Deserialize<TValue>(ref reader, options)!;

                // Add to dictionary.
                dictionary.Add(key, value);
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, IDictionary<TKey, TValue> dictionary, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach ((TKey key, TValue value) in dictionary)
            {
                string? propertyName = key.ToString();
                string? convertedName = options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName;

                writer.WritePropertyName(convertedName);
                JsonSerializer.Serialize(writer, value, options);
            }

            writer.WriteEndObject();
        }
    }
}
