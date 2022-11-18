// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Json.Annotation;

/// <summary>
/// Json 枚举类型
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal class JsonEnumAttribute : Attribute
{
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
}