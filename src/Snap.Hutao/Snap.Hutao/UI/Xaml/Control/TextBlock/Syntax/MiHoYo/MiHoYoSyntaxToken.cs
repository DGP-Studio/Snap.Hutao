// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

internal readonly ref struct MiHoYoSyntaxToken
{
    public readonly MiHoYoSyntaxTokenType Type;
    public readonly TextPosition Position;

    public MiHoYoSyntaxToken(MiHoYoSyntaxTokenType type, TextPosition position)
    {
        Type = type;
        Position = position;
    }
}