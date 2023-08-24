// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Primitives;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss.Converter;

/// <summary>
/// 圣遗物套装转换器
/// </summary>
[HighQuality]
internal sealed class ReliquarySetsConverter : JsonConverter<ReliquarySets>
{
    private const char Separator = ',';

    /// <inheritdoc/>
    public override ReliquarySets? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is { } source)
        {
            List<ReliquarySet> sets = new();
            foreach (StringSegment segment in new StringTokenizer(source, Separator.ToArray()))
            {
                if (segment is { HasValue: true, Length: > 0 })
                {
                    sets.Add(new(segment.Value));
                }
            }

            return new(sets);
        }
        else
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, ReliquarySets value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(string.Join(Separator, value));
    }
}