// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Extension;

internal static class StringExtension
{
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