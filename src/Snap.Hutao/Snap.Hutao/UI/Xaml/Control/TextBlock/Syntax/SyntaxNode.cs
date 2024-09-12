// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax;

internal abstract class SyntaxNode<TSelf, TKind>
    where TSelf : SyntaxNode<TSelf, TKind>
    where TKind : struct, Enum
{
    public SyntaxNode(TKind kind, string text, int start, int end)
    {
        Kind = kind;
        Text = text;
        Position = new(start, end);
    }

    public SyntaxNode(TKind kind, string text, in TextPosition position)
    {
        Kind = kind;
        Text = text;
        Position = position;
    }

    public TKind Kind { get; protected set; }

    public List<TSelf> Children { get; } = [];

    public TextPosition Position { get; protected set; }

    public ReadOnlySpan<char> Span { get => Text.AsSpan().Slice(Position.Start, Position.Length); }

    protected string Text { get; set; } = default!;
}