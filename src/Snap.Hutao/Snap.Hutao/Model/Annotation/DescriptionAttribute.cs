// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Annotation;

/// <summary>
/// 枚举的文本描述特性
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
internal class DescriptionAttribute : Attribute
{
    /// <summary>
    /// 构造一个新的枚举的文本描述特性
    /// </summary>
    /// <param name="description">描述</param>
    public DescriptionAttribute(string description)
    {
        Description = description;
    }

    /// <summary>
    /// 获取文本描述
    /// </summary>
    public string Description { get; init; }
}
