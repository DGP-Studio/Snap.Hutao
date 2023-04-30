// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Json.Annotation;

/// <summary>
/// Json 序列化类型
/// </summary>
[HighQuality]
internal enum JsonSerializeType
{
    /// <summary>
    /// 数字
    /// </summary>
    Number,

    /// <summary>
    /// 字符串包裹的数字
    /// </summary>
    NumberString,

    /// <summary>
    /// 名称字符串
    /// </summary>
    String,
}