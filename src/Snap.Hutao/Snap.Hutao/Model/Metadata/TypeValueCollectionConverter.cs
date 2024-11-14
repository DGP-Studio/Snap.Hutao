// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata;

internal sealed class TypeValueCollectionConverter : JsonConverterFactory
{
    private readonly Type converterType = typeof(Converter<,>);

    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(TypeValueCollection<,>).IsAssignableFrom(typeToConvert.GetGenericTypeDefinition());
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return Activator.CreateInstance(converterType.MakeGenericType(typeToConvert.GetGenericArguments())) as JsonConverter;
    }

    private sealed class Converter<TType, TValue> : JsonConverter<TypeValueCollection<TType, TValue>>
        where TType : notnull
    {
        public override TypeValueCollection<TType, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new(JsonSerializer.Deserialize<ImmutableArray<TypeValue<TType, TValue>>>(ref reader, options));
        }

        public override void Write(Utf8JsonWriter writer, TypeValueCollection<TType, TValue> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.TypeValues, options);
        }
    }
}