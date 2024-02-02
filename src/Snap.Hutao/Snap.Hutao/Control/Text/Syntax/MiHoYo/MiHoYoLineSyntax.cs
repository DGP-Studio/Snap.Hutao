// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Text.Syntax.MiHoYo;

internal sealed class MiHoYoLineSyntax : MiHoYoSyntaxNode
{
    public MiHoYoLineSyntax(bool hasTailingNewLine, string text, int start, int end)
        : base(MiHoYoSyntaxKind.Line, text, start, end)
    {
        HasTailingNewLine = hasTailingNewLine;
    }

    public bool HasTailingNewLine { get; }

    public TextPosition TextPosition { get => HasTailingNewLine ? new(Position.Start, Position.Length - 1) : Position; }
}