// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Core.Json.Converter;

/// <summary>
/// 实现日期的转换
/// </summary>
internal class DateTimeConverter : JsonConverter<DateTime>
{
    /// <inheritdoc/>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is string dataTimeString)
        {
            return DateTime.Parse(dataTimeString);
        }

        return default(DateTime);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}
