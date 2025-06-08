// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

internal abstract class MiHoYoXmlLikeElementSyntax : MiHoYoSyntaxNode
{
    protected MiHoYoXmlLikeElementSyntax(MiHoYoSyntaxKind kind, string text, int start, int end)
        : base(kind, text, start, end)
    {
    }

    protected MiHoYoXmlLikeElementSyntax(MiHoYoSyntaxKind kind, string text, in TextPosition position)
        : base(kind, text, position)
    {
    }

    public abstract TextPosition ContentPosition { get; }

    public ReadOnlySpan<char> ContentSpan { get => Text.AsSpan(ContentPosition.Start, ContentPosition.Length); }
}