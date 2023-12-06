﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string TrimEnd(this string source, string value)
    {
        return source.AsSpan().TrimEnd(value).ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool EqualsAny(this string source, in ReadOnlySpan<string> values, StringComparison comparisonType)
    {
        foreach (ref readonly string value in values)
        {
            if (source.Equals(value, comparisonType))
            {
                return true;
            }
        }

        return false;
    }
}