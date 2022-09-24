// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Json.Converter;

/// <summary>
/// 枚举 - 字符串数字 转换器
/// </summary>
/// <typeparam name="TEnum">枚举的类型</typeparam>
internal class EnumStringValueConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    /// <inheritdoc/>
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is string str)
        {
            return Enum.Parse<TEnum>(str);
        }

        throw Must.NeverHappen();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("D"));
    }
}