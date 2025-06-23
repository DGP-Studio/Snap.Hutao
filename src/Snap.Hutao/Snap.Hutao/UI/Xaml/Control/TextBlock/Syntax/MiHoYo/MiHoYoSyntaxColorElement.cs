// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

internal sealed class MiHoYoSyntaxColorElement : MiHoYoSyntaxElement
{
    public MiHoYoSyntaxColorElement(TextPosition fullPosition, TextPosition colorPosition, ImmutableArray<MiHoYoSyntaxElement> children)
        : base(fullPosition, children)
    {
        ColorPosition = colorPosition;
    }

    public TextPosition ColorPosition { get; }

    public ReadOnlySpan<char> GetColorSpan(ReadOnlySpan<char> source)
    {
        return source.Slice(ColorPosition.Start, ColorPosition.Length);
    }
}