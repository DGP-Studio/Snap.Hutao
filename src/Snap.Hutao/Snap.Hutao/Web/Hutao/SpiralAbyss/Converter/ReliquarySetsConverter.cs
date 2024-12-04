// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.SpiralAbyss.Converter;

internal sealed class ReliquarySetsConverter : JsonConverter<ReliquarySets>
{
    public override ReliquarySets? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is { } source)
        {
            List<ReliquarySet> sets = [];
            ReadOnlySpan<char> sourceSpan = source.AsSpan();
            foreach (Range range in sourceSpan.Split(','))
            {
                ReadOnlySpan<char> target = sourceSpan[range];
                if (!target.IsEmpty)
                {
                    sets.Add(new(target));
                }
            }

            return new(sets);
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, ReliquarySets value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(string.Join(',', value));
    }
}