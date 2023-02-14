// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExpressionService;
using Snap.Hutao.Core.Json.Annotation;

namespace Snap.Hutao.Core.Json.Converter;

/// <summary>
/// 枚举转换器
/// </summary>
/// <typeparam name="TEnum">枚举的类型</typeparam>
[HighQuality]
internal sealed class ConfigurableEnumConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    private readonly JsonSerializeType readAs;
    private readonly JsonSerializeType writeAs;

    /// <summary>
    /// 构造一个新的枚举转换器
    /// </summary>
    /// <param name="readAs">读取</param>
    /// <param name="writeAs">写入</param>
    public ConfigurableEnumConverter(JsonSerializeType readAs, JsonSerializeType writeAs)
    {
        this.readAs = readAs;
        this.writeAs = writeAs;
    }

    /// <inheritdoc/>
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (readAs == JsonSerializeType.Int32)
        {
            return CastTo<TEnum>.From(reader.GetInt32());
        }

        if (reader.GetString() is string str)
        {
            return Enum.Parse<TEnum>(str);
        }

        throw Must.NeverHappen();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        switch (writeAs)
        {
            case JsonSerializeType.Int32:
                writer.WriteNumberValue(CastTo<int>.From(value));
                break;
            case JsonSerializeType.Int32AsString:
                writer.WriteStringValue(value.ToString("D"));
                break;
            default:
                writer.WriteStringValue(value.ToString());
                break;
        }
    }
}