// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Text;

namespace Snap.Hutao.Extension;

/// <summary>
/// 字符串拓展
/// </summary>
[HighQuality]
internal static class StringExtension
{
    /// <summary>
    /// 转换到Uri
    /// </summary>
    /// <param name="value">字符串</param>
    /// <returns>Uri</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Uri ToUri(this string value)
    {
        return new(value);
    }
}