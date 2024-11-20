// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Extension;

internal static class StringExtension
{
    public static bool EqualsAny(this string value, ReadOnlySpan<string> values, StringComparison stringComparison)
    {
        foreach (ref readonly string item in values)
        {
            if (value.Equals(item, stringComparison))
            {
                return true;
            }
        }

        return false;
    }

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
}