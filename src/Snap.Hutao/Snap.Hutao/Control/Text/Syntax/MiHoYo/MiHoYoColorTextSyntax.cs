// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Text.Syntax.MiHoYo;

internal sealed class MiHoYoColorTextSyntax : MiHoYoXmlElementSyntax
{
    public MiHoYoColorTextSyntax(MiHoYoColorKind colorKind, string text, int start, int end)
        : base(MiHoYoSyntaxKind.ColorText, text, start, end)
    {
        ColorKind = colorKind;
    }

    public MiHoYoColorTextSyntax(MiHoYoColorKind colorKind, string text, in TextPosition position)
        : base(MiHoYoSyntaxKind.ColorText, text, position)
    {
        ColorKind = colorKind;
    }

    public MiHoYoColorKind ColorKind { get; }

    public override TextPosition ContentPosition
    {
        get
        {
            return ColorKind switch
            {
                MiHoYoColorKind.Rgba => new(Position.Start + 17, Position.End - 8),
                MiHoYoColorKind.Rgb => new(Position.Start + 15, Position.End - 8),
                _ => throw Must.NeverHappen(),
            };
        }
    }

    public TextPosition ColorPosition
    {
        get
        {
            return ColorKind switch
            {
                MiHoYoColorKind.Rgba => new(Position.Start + 8, Position.Start + 16),
                MiHoYoColorKind.Rgb => new(Position.Start + 8, Position.Start + 14),
                _ => throw Must.NeverHappen(),
            };
        }
    }

    public ReadOnlySpan<char> ColorSpan { get => Text.AsSpan()[ColorPosition.Start..ColorPosition.End]; }
}