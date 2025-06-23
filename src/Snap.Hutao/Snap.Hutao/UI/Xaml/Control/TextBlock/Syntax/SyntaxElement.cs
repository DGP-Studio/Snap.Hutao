// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax;

internal abstract class SyntaxElement<TSelf>
    where TSelf : SyntaxElement<TSelf>
{
    protected SyntaxElement(TextPosition position, ImmutableArray<TSelf> children)
    {
        Position = position;
        Children = children;
    }

    public ImmutableArray<TSelf> Children { get; }

    public TextPosition Position { get; }

    public ReadOnlySpan<char> GetSpan(ReadOnlySpan<char> source)
    {
        return source.Slice(Position.Start, Position.Length);
    }
}