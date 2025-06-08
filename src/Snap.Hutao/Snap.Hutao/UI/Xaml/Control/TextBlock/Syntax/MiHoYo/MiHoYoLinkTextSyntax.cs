// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

internal sealed class MiHoYoLinkTextSyntax : MiHoYoXmlLikeElementSyntax
{
    private readonly int idLength;

    public MiHoYoLinkTextSyntax(MiHoYoLinkKind linkKind, int idLength, string text, int start, int end)
        : base(MiHoYoSyntaxKind.LinkText, text, start, end)
    {
        LinkKind = linkKind;
        this.idLength = idLength;
    }

    public MiHoYoLinkTextSyntax(MiHoYoLinkKind linkKind, int idLength, string text, in TextPosition position)
        : base(MiHoYoSyntaxKind.LinkText, text, in position)
    {
        LinkKind = linkKind;
        this.idLength = idLength;
    }

    public MiHoYoLinkKind LinkKind { get; }

    public override TextPosition ContentPosition
    {
        get => new(Position.Start + 7 + idLength + 1, Position.End - 7);
    }

    public TextPosition IdPosition
    {
        get => new(Position.Start + 7, Position.Start + 7 + idLength);
    }

    public ReadOnlySpan<char> IdSpan { get => Text.AsSpan()[IdPosition.Start..IdPosition.End]; }
}