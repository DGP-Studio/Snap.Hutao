// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text;

namespace Snap.Hutao.Extension;

/// <summary>
/// <see cref="StringBuilder"/> 扩展方法
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    /// 当条件符合时执行 <see cref="StringBuilder.Append(string?)"/>
    /// </summary>
    /// <param name="sb">字符串建造器</param>
    /// <param name="condition">条件</param>
    /// <param name="value">附加的字符</param>
    /// <returns>同一个字符串建造器</returns>
    public static StringBuilder AppendIf(this StringBuilder sb, bool condition, char? value)
    {
        return condition ? sb.Append(value) : sb;
    }

    /// <summary>
    /// 当条件符合时执行 <see cref="StringBuilder.Append(string?)"/>
    /// </summary>
    /// <param name="sb">字符串建造器</param>
    /// <param name="condition">条件</param>
    /// <param name="value">附加的字符串</param>
    /// <returns>同一个字符串建造器</returns>
    public static StringBuilder AppendIf(this StringBuilder sb, bool condition, string? value)
    {
        return condition ? sb.Append(value) : sb;
    }

    /// <summary>
    /// 当条件符合时执行 <see cref="StringBuilder.Append(string?)"/>
    /// </summary>
    /// <param name="sb">字符串建造器</param>
    /// <param name="condition">条件</param>
    /// <param name="trueValue">条件符合时附加的字符串</param>
    /// <param name="falseValue">条件不符合时附加的字符串</param>
    /// <returns>同一个字符串建造器</returns>
    public static StringBuilder AppendIfElse(this StringBuilder sb, bool condition, string? trueValue, string? falseValue)
    {
        return condition ? sb.Append(trueValue) : sb.Append(falseValue);
    }
}