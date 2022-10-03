// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Json.Converter;

/// <summary>
/// Json字典转换器
/// </summary>
public class StringEnumKeyDictionaryConverter : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        if (typeToConvert.GetGenericTypeDefinition() != typeof(IDictionary<,>))
        {
            return false;
        }

        return typeToConvert.GetGenericArguments()[0].IsEnum;
    }

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
    {
        Type keyType = type.GetGenericArguments()[0];
        Type valueType = type.GetGenericArguments()[1];

        Type innerConverterType = typeof(StringEnumDictionaryConverterInner<,>).MakeGenericType(keyType, valueType);
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

                if (!Enum.TryParse(propertyName, ignoreCase: false, out TKey key) && !Enum.TryParse(propertyName, ignoreCase: true, out key))
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
