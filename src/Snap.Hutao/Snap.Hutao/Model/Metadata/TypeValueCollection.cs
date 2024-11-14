// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata;

[JsonConverter(typeof(ConverterFactory))]
internal sealed class TypeValueCollection<TType, TValue>
    where TType : notnull
{
    private readonly SortedDictionary<TType, TValue> typeValues = [];

    public TypeValueCollection(ImmutableArray<TypeValue<TType, TValue>> entries)
    {
        foreach (ref readonly TypeValue<TType, TValue> entry in entries.AsSpan())
        {
            typeValues.Add(entry.Type, entry.Value);
        }
    }

    internal IReadOnlyDictionary<TType, TValue> TypeValues { get => typeValues; }

    public TValue? GetValueOrDefault(TType type)
    {
        typeValues.TryGetValue(type, out TValue? value);
        return value;
    }
}

[SuppressMessage("", "SA1402")]
file sealed class ConverterFactory : JsonConverterFactory
{
    private static readonly Type converterType = typeof(Converter<,>);

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(TypeValueCollection<,>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return Activator.CreateInstance(converterType.MakeGenericType(typeToConvert.GetGenericArguments())) as JsonConverter;
    }
}

[SuppressMessage("", "SA1402")]
file sealed class Converter<TType, TValue> : JsonConverter<TypeValueCollection<TType, TValue>>
    where TType : notnull
{
    public override TypeValueCollection<TType, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new(JsonSerializer.Deserialize<ImmutableArray<TypeValue<TType, TValue>>>(ref reader, options));
    }

    public override void Write(Utf8JsonWriter writer, TypeValueCollection<TType, TValue> value, JsonSerializerOptions options)
    {
        throw new JsonException();
    }
}