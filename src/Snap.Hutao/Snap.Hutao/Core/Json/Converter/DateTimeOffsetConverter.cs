// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;

namespace Snap.Hutao.Core.Json.Converter;

/// <summary>
/// 实现日期的转换
/// 此转换器无法实现无损往返
/// 必须在反序列化后调整 Offset
/// </summary>
[HighQuality]
internal class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    private const string Format = "yyyy-MM-dd HH:mm:ss";

    /// <inheritdoc/>
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is { } dataTimeString)
        {
            // By doing so, the DateTimeOffset parsed out will be a
            // no offset datetime, and need to be adjusted later
            DateTime dateTime = DateTime.ParseExact(dataTimeString, Format, CultureInfo.InvariantCulture);
            return new DateTimeOffset(dateTime, default);
        }

        return default;
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.DateTime.ToString(Format, CultureInfo.InvariantCulture));
    }
}