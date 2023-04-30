// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Json.Converter;
using System.Text.Json.Serialization.Metadata;

namespace Snap.Hutao.Core.Json.Annotation;

/// <summary>
/// Json 枚举类型
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal class JsonEnumAttribute : Attribute
{
    private static readonly Type UnsafeEnumConverterType = typeof(UnsafeEnumConverter<>);

    /// <summary>
    /// 构造一个新的Json枚举声明
    /// </summary>
    /// <param name="readAndWriteAs">读取与写入</param>
    public JsonEnumAttribute(JsonSerializeType readAndWriteAs)
    {
        ReadAs = readAndWriteAs;
        WriteAs = readAndWriteAs;
    }

    /// <summary>
    /// 构造一个新的Json枚举声明
    /// </summary>
    /// <param name="readAs">读取</param>
    /// <param name="writeAs">写入</param>
    public JsonEnumAttribute(JsonSerializeType readAs, JsonSerializeType writeAs)
    {
        ReadAs = readAs;
        WriteAs = writeAs;
    }

    /// <summary>
    /// 读取形式
    /// </summary>
    public JsonSerializeType ReadAs { get; init; }

    /// <summary>
    /// 写入形式
    /// </summary>
    public JsonSerializeType WriteAs { get; init; }

    /// <summary>
    /// 创建一个新的转换器
    /// </summary>
    /// <param name="info">属性信息</param>
    /// <returns>Json转换器</returns>
    internal JsonConverter CreateConverter(JsonPropertyInfo info)
    {
        Type converterType = UnsafeEnumConverterType.MakeGenericType(info.PropertyType);
        return (JsonConverter)Activator.CreateInstance(converterType, ReadAs, WriteAs)!;
    }
}