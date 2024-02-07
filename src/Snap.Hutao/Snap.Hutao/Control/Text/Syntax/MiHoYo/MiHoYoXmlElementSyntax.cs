// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Text.Syntax.MiHoYo;

internal abstract class MiHoYoXmlElementSyntax : MiHoYoSyntaxNode
{
    public MiHoYoXmlElementSyntax(MiHoYoSyntaxKind kind, string text, int start, int end)
        : base(kind, text, start, end)
    {
    }

    public MiHoYoXmlElementSyntax(MiHoYoSyntaxKind kind, string text, in TextPosition position)
        : base(kind, text, position)
    {
    }

    public abstract TextPosition ContentPosition { get; }

    public ReadOnlySpan<char> ContentSpan { get => Text.AsSpan(ContentPosition.Start, ContentPosition.Length); }
}