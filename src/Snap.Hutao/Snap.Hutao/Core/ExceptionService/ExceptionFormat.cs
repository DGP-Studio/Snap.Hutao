// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections;
using System.Text;

namespace Snap.Hutao.Core.ExceptionService;

/// <summary>
/// 异常格式化
/// </summary>
internal sealed class ExceptionFormat
{
    private static readonly string SectionSeparator = new('-', 40);

    /// <summary>
    /// 格式化异常
    /// </summary>
    /// <param name="exception">异常</param>
    /// <returns>格式化后的异常</returns>
    public static string Format(Exception exception)
    {
        StringBuilder builder = new();
        builder.AppendLine("Exception Data:");

        foreach (DictionaryEntry entry in exception.Data)
        {
            builder
                .Append(entry.Key)
                .Append(':')
                .Append(entry.Value)
                .Append(StringLiterals.CRLF);
        }

        builder.AppendLine(SectionSeparator);
        builder.Append(exception.ToString());

        return builder.ToString();
    }
}