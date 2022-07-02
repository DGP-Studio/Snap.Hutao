// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Core.Json.Converter;

/// <summary>
/// Json字典转换器
/// </summary>
/// <typeparam name="TKeyConverter">键的类型</typeparam>
public class StringEnumKeyDictionaryConverter : JsonConverterFactory
{
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
        Type keyType = type.GetGenericArguments()[0];
        Type valueType = type.GetGenericArguments()[1];

        Type innerConverterType = typeof(StringEnumDictionaryConverterInner<,>).MakeGenericType(keyType, valueType);
        JsonConverter converter = (JsonConverter)Activator.CreateInstance(innerConverterType, BindingFlags.Instance | BindingFlags.Public, null, new object[] { options }, null)!;
        return converter;
    }

    private class StringEnumDictionaryConverterInner<TKey, TValue> : JsonConverter<IDictionary<TKey, TValue>>
        where TKey : struct, Enum
    {
        private readonly JsonConverter<TValue>? valueConverter;
        private readonly Type keyType;
        private readonly Type valueType;

        public StringEnumDictionaryConverterInner(JsonSerializerOptions options)
        {
            valueConverter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue));

            // Cache the key and value types.
            keyType = typeof(TKey);
            valueType = typeof(TValue);
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

                if (!Enum.TryParse(propertyName, out TKey key))
                {
                    throw new JsonException($"Unable to convert \"{propertyName}\" to Enum \"{keyType}\".");
                }

                // Get the value.
                TValue value;
                if (valueConverter != null)
                {
                    reader.Read();
                    value = valueConverter.Read(ref reader, valueType, options)!;
                }
                else
                {
                    value = JsonSerializer.Deserialize<TValue>(ref reader, options)!;
                }

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

                if (valueConverter != null)
                {
                    valueConverter.Write(writer, value, options);
                }
                else
                {
                    JsonSerializer.Serialize(writer, value, options);
                }
            }

            writer.WriteEndObject();
        }
    }
}
