// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

internal sealed class MiHoYoSyntaxItalicElement : MiHoYoSyntaxElement
{
    public MiHoYoSyntaxItalicElement(TextPosition position, ImmutableArray<MiHoYoSyntaxElement> children)
        : base(position, children)
    {
    }
}