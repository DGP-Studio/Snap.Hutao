// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core;

/// <summary>
/// 字符串字面量
/// </summary>
[HighQuality]
internal static class StringLiterals
{
    /// <summary>
    /// 1
    /// </summary>
    public const string One = "1";

    /// <summary>
    /// 1.1
    /// </summary>
    public const string OnePointOne = "1.1";

    /// <summary>
    /// True
    /// </summary>
    public const string True = "True";

    /// <summary>
    /// False
    /// </summary>
    public const string False = "False";

    /// <summary>
    /// CRLF 换行符
    /// </summary>
    public const string CRLF = "\r\n";

    /// <summary>
    /// 获取小写的布尔字符串
    /// </summary>
    /// <param name="value">值</param>
    /// <returns>小写布尔字符串</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string LowerBoolean(bool value)
    {
        return value ? "true" : "false";
    }
}