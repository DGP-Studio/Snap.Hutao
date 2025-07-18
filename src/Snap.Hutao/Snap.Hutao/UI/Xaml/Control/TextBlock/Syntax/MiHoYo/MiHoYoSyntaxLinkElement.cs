// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using System.Collections.Immutable;

namespace Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

internal sealed class MiHoYoSyntaxLinkElement : MiHoYoSyntaxElement
{
    public MiHoYoSyntaxLinkElement(TextPosition position, TextPosition idPosition, ImmutableArray<MiHoYoSyntaxElement> children)
        : base(position, children)
    {
        IdPosition = idPosition;
    }

    public TextPosition IdPosition { get; }

    public MiHoYoSyntaxLinkKind GetLinkKind(ReadOnlySpan<char> source)
    {
        return source[IdPosition.Start] switch
        {
            'P' => MiHoYoSyntaxLinkKind.Inherent,
            'N' => MiHoYoSyntaxLinkKind.Name,
            'S' => MiHoYoSyntaxLinkKind.Skill,
            _ => throw HutaoException.Throw($"Unexpected link kind :{source[IdPosition.Start]}"),
        };
    }

    public ReadOnlySpan<char> GetIdSpan(ReadOnlySpan<char> source)
    {
        return source.Slice(IdPosition.Start, IdPosition.Length);
    }
}