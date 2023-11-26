// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Text;

namespace Snap.Hutao.Extension;

/// <summary>
/// <see cref="StringBuilder"/> 扩展方法
/// </summary>
[HighQuality]
internal static class StringBuilderExtension
{
    /// <summary>
    /// 当条件符合时执行 <see cref="StringBuilder.Append(string?)"/>
    /// </summary>
    /// <param name="sb">字符串建造器</param>
    /// <param name="condition">条件</param>
    /// <param name="value">附加的字符</param>
    /// <returns>同一个字符串建造器</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder AppendIf(this StringBuilder sb, bool condition, string? value)
    {
        return condition ? sb.Append(value) : sb;
    }

    public static string ToStringTrimEndReturn(this StringBuilder builder)
    {
        Must.Argument(builder.Length >= 1, "StringBuilder 的长度必须大于 0");
        int remove = 0;
        if (builder[^1] is '\n')
        {
            remove = 1;

            if (builder.Length >= 2 && builder[^2] is '\r')
            {
                remove = 2;
            }
        }

        return builder.ToString(0, builder.Length - remove);
    }
}