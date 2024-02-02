// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Text.Syntax.MiHoYo;

internal sealed class MiHoYoPlainTextSyntax : MiHoYoSyntaxNode
{
    public MiHoYoPlainTextSyntax(string text, int start, int end)
        : base(MiHoYoSyntaxKind.PlainText, text, start, end)
    {
    }
}