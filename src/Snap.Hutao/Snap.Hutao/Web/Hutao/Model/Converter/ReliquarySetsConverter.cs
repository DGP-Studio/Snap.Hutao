using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Snap.Hutao.Web.Hutao.Model.Converter;

/// <summary>
/// 圣遗物套装转换器
/// </summary>
internal class ReliquarySetsConverter : JsonConverter<ReliquarySets>
{
    /// <inheritdoc/>
    public override ReliquarySets? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? source = reader.GetString();

        if (source != null)
        {
            string[] sets = source.Split(';');
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
        writer.WriteStringValue(string.Join(';', value));
    }
}
