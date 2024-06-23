// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

internal sealed class MiHoYoRootSyntax : MiHoYoSyntaxNode
{
    public MiHoYoRootSyntax(string text, int start, int end)
        : base(MiHoYoSyntaxKind.Root, text, start, end)
    {
    }
}