// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Numerics;

namespace Snap.Hutao.Extension;

internal static class SpanExtension
{
    public static int IndexOfMax<T>(this in ReadOnlySpan<T> span)
        where T : INumber<T>, IMinMaxValue<T>
    {
        T max = T.MinValue;
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

    public static byte Average(this in ReadOnlySpan<byte> span)
    {
        if (span.IsEmpty)
        {
            return 0;
        }

        int sum = 0;
        int count = 0;
        foreach (ref readonly byte b in span)
        {
            sum += b;
            count++;
        }

        // ReSharper disable once IntDivisionByZero
        return unchecked((byte)(sum / count));
    }
}