// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model.Converter;

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
        if (reader.GetString() is string source)
        {
            string[] sets = source.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            return new(sets.Select(set => new ReliquarySet(set)));
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