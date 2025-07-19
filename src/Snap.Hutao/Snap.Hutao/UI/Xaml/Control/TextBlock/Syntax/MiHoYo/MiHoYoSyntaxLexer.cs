// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

internal ref struct MiHoYoSyntaxLexer
{
    public readonly ReadOnlySpan<char> Input;
    private int position;

    public MiHoYoSyntaxLexer(ReadOnlySpan<char> input)
    {
        Input = input;
    }

    public MiHoYoSyntaxToken Next()
    {
        if (position >= Input.Length)
        {
            return new(MiHoYoSyntaxTokenKind.EndOfFile, new(position, position));
        }

        int start = position;

        if (Match("{LINK#"))
        {
            if (ReadUntil('}', out int idEnd))
            {
                return new(MiHoYoSyntaxTokenKind.LinkOpen, new(start, idEnd + 1));
            }
        }

        if (Match("{/LINK}"))
        {
            return new(MiHoYoSyntaxTokenKind.LinkClose, new(start, position));
        }

        if (Match("{SPRITE_PRESET#"))
        {
            if (ReadUntil('}', out int idEnd))
            {
                return new(MiHoYoSyntaxTokenKind.SpritePreset, new(start, idEnd + 1));
            }
        }

        if (Match("<color="))
        {
            if (ReadUntil('>', out int end))
            {
                return new(MiHoYoSyntaxTokenKind.ColorOpen, new(start, end + 1));
            }
        }

        if (Match("</color>"))
        {
            return new(MiHoYoSyntaxTokenKind.ColorClose, new(start, position));
        }

        if (Match("<i>"))
        {
            return new(MiHoYoSyntaxTokenKind.ItalicOpen, new(start, position));
        }

        if (Match("</i>"))
        {
            return new(MiHoYoSyntaxTokenKind.ItalicClose, new(start, position));
        }

        while (position < Input.Length && !IsSpecialStart(Input[position]))
        {
            position++;
        }

        return new(MiHoYoSyntaxTokenKind.Text, new(start, position));
    }

    private static bool IsSpecialStart(char c)
    {
        return c is '{' or '<';
    }

    private bool Match(string keyword)
    {
        ReadOnlySpan<char> slice = Input[position..];
        if (slice.StartsWith(keyword.AsSpan(), StringComparison.Ordinal))
        {
            position += keyword.Length;
            return true;
        }

        return false;
    }

    private bool ReadUntil(char end, out int endPos)
    {
        int relativeIndex = Input[position..].IndexOf(end);
        if (relativeIndex >= 0)
        {
            endPos = position + relativeIndex;
            position = endPos + 1;
            return true;
        }

        endPos = -1;
        return false;
    }
}