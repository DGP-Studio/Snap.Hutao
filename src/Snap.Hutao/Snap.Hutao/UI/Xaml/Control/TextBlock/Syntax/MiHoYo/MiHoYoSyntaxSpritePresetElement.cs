// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

internal sealed class MiHoYoSyntaxSpritePresetElement : MiHoYoSyntaxElement
{
    public MiHoYoSyntaxSpritePresetElement(TextPosition position, TextPosition idPosition, ImmutableArray<MiHoYoSyntaxElement> children)
        : base(position, children)
    {
        IdPosition = idPosition;
    }

    public TextPosition IdPosition { get; }

    public ReadOnlySpan<char> GetIdSpan(ReadOnlySpan<char> source)
    {
        return source.Slice(IdPosition.Start, IdPosition.Length);
    }
}