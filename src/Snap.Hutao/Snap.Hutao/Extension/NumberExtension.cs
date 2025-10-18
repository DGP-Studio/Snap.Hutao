// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Extension;

internal static class NumberExtension
{
    private static readonly KeyValuePair<string, int>[] RomanNumeralsSequence =
    [
        KeyValuePair.Create("M", 1000),
        KeyValuePair.Create("CM", 900),
        KeyValuePair.Create("D", 500),
        KeyValuePair.Create("CD", 400),
        KeyValuePair.Create("C", 100),
        KeyValuePair.Create("XC", 90),
        KeyValuePair.Create("L", 50),
        KeyValuePair.Create("XL", 40),
        KeyValuePair.Create("X", 10),
        KeyValuePair.Create("IX", 9),
        KeyValuePair.Create("V", 5),
        KeyValuePair.Create("IV", 4),
        KeyValuePair.Create("I", 1),
    ];

    public static string ToRoman(this int input)
    {
        const int MinValue = 1;
        const int MaxValue = 3999;
        const int MaxRomanNumeralLength = 15;

        if (input is < MinValue or > MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(input));
        }

        Span<char> builder = stackalloc char[MaxRomanNumeralLength];
        int pos = 0;

        foreach (KeyValuePair<string, int> pair in RomanNumeralsSequence)
        {
            while (input >= pair.Value)
            {
                pair.Key.AsSpan().CopyTo(builder[pos..]);
                pos += pair.Key.Length;
                input -= pair.Value;
            }
        }

        return builder[..pos].ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint StringLength(in this uint x)
    {
        // Benchmarked and compared as most optimized solution
        return (uint)(MathF.Log10(x) + 1);
    }
}