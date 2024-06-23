﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Primitives;
using System.Globalization;

namespace Snap.Hutao.Core.Json.Converter;

[HighQuality]
internal sealed class SeparatorCommaInt32EnumerableConverter : JsonConverter<IEnumerable<int>>
{
    private const char Comma = ',';

    public override IEnumerable<int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is { } source)
        {
            return EnumerateNumbers(source);
        }

        return [];
    }

    public override void Write(Utf8JsonWriter writer, IEnumerable<int> value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(string.Join(Comma, value));
    }

    private static IEnumerable<int> EnumerateNumbers(string source)
    {
        foreach (StringSegment id in new StringTokenizer(source, [Comma]))
        {
            yield return int.Parse(id.AsSpan(), CultureInfo.CurrentCulture);
        }
    }
}