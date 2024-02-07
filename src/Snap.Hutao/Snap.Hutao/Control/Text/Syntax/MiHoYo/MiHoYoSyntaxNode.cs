// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Text.Syntax.MiHoYo;

internal abstract class MiHoYoSyntaxNode : SyntaxNode<MiHoYoSyntaxNode, MiHoYoSyntaxKind>
{
    public MiHoYoSyntaxNode(MiHoYoSyntaxKind kind, string text, int start, int end)
        : base(kind, text, start, end)
    {
    }

    public MiHoYoSyntaxNode(MiHoYoSyntaxKind kind, string text, in TextPosition position)
        : base(kind, text, position)
    {
    }
}