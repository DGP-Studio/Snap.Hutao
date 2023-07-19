// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Numerics;

namespace Snap.Hutao.Extension;

/// <summary>
/// Span 拓展
/// </summary>
internal static class SpanExtension
{
    /// <summary>
    /// 获取最大值的下标
    /// </summary>
    /// <typeparam name="T">值的类型</typeparam>
    /// <param name="span">Span</param>
    /// <returns>最大值的下标</returns>
    public static int IndexOfMax<T>(this in ReadOnlySpan<T> span)
        where T : INumber<T>
    {
        T max = T.Zero;
        int maxIndex = 0;
        for (int i = 0; i < span.Length; i++)
        {
            ref readonly T current = ref span[i];
            if (current > max)
            {
                maxIndex = i;
                max = current;
            }
        }

        return maxIndex;
    }

    public static bool TrySplitIntoTwo<T>(this in ReadOnlySpan<T> span, T separator, out ReadOnlySpan<T> left, out ReadOnlySpan<T> right)
        where T : IEquatable<T>?
    {
        int indexOfSeparator = span.IndexOf(separator);

        if (indexOfSeparator > 0)
        {
            left = span[..indexOfSeparator];
            right = span[(indexOfSeparator + 1)..];

            return true;
        }

        left = default;
        right = default;
        return false;
    }
}