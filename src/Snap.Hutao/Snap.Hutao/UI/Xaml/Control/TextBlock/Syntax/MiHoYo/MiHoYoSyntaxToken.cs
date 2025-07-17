// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

internal readonly ref struct MiHoYoSyntaxToken
{
    public readonly MiHoYoSyntaxTokenKind Kind;
    public readonly TextPosition Position;

    public MiHoYoSyntaxToken(MiHoYoSyntaxTokenKind kind, TextPosition position)
    {
        Kind = kind;
        Position = position;
    }
}