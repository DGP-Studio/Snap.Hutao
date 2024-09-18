// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using System.Text;

namespace Snap.Hutao.Extension;

internal static class StringBuilderExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder AppendIf(this StringBuilder sb, bool condition, char? value)
    {
        return condition ? sb.Append(value) : sb;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringBuilder AppendIf(this StringBuilder sb, bool condition, string? value)
    {
        return condition ? sb.Append(value) : sb;
    }

    public static string ToStringTrimEnd(this StringBuilder builder)
    {
        int index = builder.Length - 1;
        while (index >= 0 && char.IsWhiteSpace(builder[index]))
        {
            index--;
        }

        if (index < 0)
        {
            return string.Empty;
        }

        return builder.ToString(0, index + 1);
    }

    public static string ToStringTrimEndNewLine(this StringBuilder builder)
    {
        int length = builder.Length;
        int index = length - 1;

        while (index >= 0 && (char.IsWhiteSpace(builder[index]) || builder[index] == '\n' || builder[index] == '\r'))
        {
            index--;
        }

        if (index < 0)
        {
            return string.Empty;
        }

        return builder.ToString(0, index + 1);
    }
}