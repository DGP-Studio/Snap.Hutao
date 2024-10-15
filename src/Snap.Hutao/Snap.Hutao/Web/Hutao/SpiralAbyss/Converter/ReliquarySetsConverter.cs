// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Primitives;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss.Converter;

internal sealed class ReliquarySetsConverter : JsonConverter<ReliquarySets>
{
    private const char Separator = ',';

    public override ReliquarySets? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is { } source)
        {
            List<ReliquarySet> sets = [];
            foreach (StringSegment segment in new StringTokenizer(source, [Separator]))
            {
                if (segment is { HasValue: true, Length: > 0 })
                {
                    sets.Add(new(segment.Value));
                }
            }

            return new(sets);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, ReliquarySets value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(string.Join(Separator, value));
    }
}