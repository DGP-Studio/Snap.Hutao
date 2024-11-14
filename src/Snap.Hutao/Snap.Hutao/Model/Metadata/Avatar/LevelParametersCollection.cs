// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Avatar;

[JsonConverter(typeof(ConverterFactory))]
internal sealed class LevelParametersCollection<TLevel, TParameter>
    where TLevel : notnull
{
    private readonly SortedDictionary<TLevel, ImmutableArray<TParameter>> levelParameters = [];

    public LevelParametersCollection(ImmutableArray<LevelParameters<TLevel, TParameter>> entries)
    {
        foreach (ref readonly LevelParameters<TLevel, TParameter> entry in entries.AsSpan())
        {
            levelParameters.Add(entry.Level, entry.Parameters);
        }
    }

    public int Count => levelParameters.Count;

    public ImmutableArray<TParameter> this[TLevel level]
    {
        get => levelParameters[level];
    }

    internal IReadOnlyDictionary<TLevel, ImmutableArray<TParameter>> LevelParameters { get => levelParameters; }
}

[SuppressMessage("", "SA1402")]
file sealed class ConverterFactory : JsonConverterFactory
{
    private static readonly Type converterType = typeof(Converter<,>);

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(LevelParametersCollection<,>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return Activator.CreateInstance(converterType.MakeGenericType(typeToConvert.GetGenericArguments())) as JsonConverter;
    }
}

[SuppressMessage("", "SA1402")]
file sealed class Converter<TLevel, TParameter> : JsonConverter<LevelParametersCollection<TLevel, TParameter>>
    where TLevel : notnull
{
    public override LevelParametersCollection<TLevel, TParameter> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new(JsonSerializer.Deserialize<ImmutableArray<LevelParameters<TLevel, TParameter>>>(ref reader, options));
    }

    public override void Write(Utf8JsonWriter writer, LevelParametersCollection<TLevel, TParameter> value, JsonSerializerOptions options)
    {
        throw new JsonException();
    }
}