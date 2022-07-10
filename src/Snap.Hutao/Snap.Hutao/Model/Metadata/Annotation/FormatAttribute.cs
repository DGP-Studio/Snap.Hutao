// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Annotation;

/// <summary>
/// 格式特性
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
internal class FormatAttribute : Attribute
{
    /// <summary>
    /// 指示该字段应用的格式化方法
    /// </summary>
    /// <param name="method">格式化方法</param>
    public FormatAttribute(FormatMethod method)
    {
        Method = method;
    }

    /// <summary>
    /// 格式化方法
    /// </summary>
    public FormatMethod Method { get; init; }
}