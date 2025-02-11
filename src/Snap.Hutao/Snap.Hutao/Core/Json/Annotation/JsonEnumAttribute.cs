// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Converter;
using System.Text.Json.Serialization.Metadata;

namespace Snap.Hutao.Core.Json.Annotation;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class JsonEnumAttribute : Attribute
{
    private static readonly Type UnsafeEnumConverterType = typeof(UnsafeEnumConverter<>);

    private readonly JsonEnumSerializeHandling readAs;
    private readonly JsonEnumSerializeHandling writeAs;

    public JsonEnumAttribute(JsonEnumSerializeHandling readAndWriteAs)
    {
        readAs = readAndWriteAs;
        writeAs = readAndWriteAs;
    }

    public JsonEnumAttribute(JsonEnumSerializeHandling readAs, JsonEnumSerializeHandling writeAs)
    {
        this.readAs = readAs;
        this.writeAs = writeAs;
    }

    internal JsonConverter CreateConverter(JsonPropertyInfo info)
    {
        Type converterType = UnsafeEnumConverterType.MakeGenericType(info.PropertyType);
        JsonConverter? converter = Activator.CreateInstance(converterType, readAs, writeAs) as JsonConverter;
        ArgumentNullException.ThrowIfNull(converter);
        return converter;
    }
}