// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using System.Collections.Immutable;

namespace Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

internal ref struct MiHoYoSyntaxParser
{
    private MiHoYoSyntaxLexer lexer;
    private MiHoYoSyntaxToken current;

    public MiHoYoSyntaxParser(MiHoYoSyntaxLexer lexer)
    {
        this.lexer = lexer;
    }

    public ImmutableArray<MiHoYoSyntaxElement> Parse()
    {
        current = lexer.Next();

        ImmutableArray<MiHoYoSyntaxElement>.Builder builder = ImmutableArray.CreateBuilder<MiHoYoSyntaxElement>();
        while (current.Type is not MiHoYoSyntaxTokenType.EndOfFile)
        {
            builder.Add(ParseElement());
        }

        return builder.ToImmutable();
    }

    private void NextToken()
    {
        current = lexer.Next();
    }

    private MiHoYoSyntaxElement ParseElement()
    {
        return current.Type switch
        {
            MiHoYoSyntaxTokenType.Text => ParseText(),
            MiHoYoSyntaxTokenType.ItalicOpen => ParseItalic(),
            MiHoYoSyntaxTokenType.ColorOpen => ParseColor(),
            MiHoYoSyntaxTokenType.LinkOpen => ParseLink(),
            _ => throw HutaoException.Throw($"Unexpected token: {current.Type}"),
        };
    }

    private MiHoYoSyntaxElement ParseText()
    {
        MiHoYoSyntaxTextElement element = new(current.Position, default);
        NextToken();
        return element;
    }

    private MiHoYoSyntaxElement ParseItalic()
    {
        int start = current.Position.Start;
        NextToken();

        ImmutableArray<MiHoYoSyntaxElement>.Builder children =  ImmutableArray.CreateBuilder<MiHoYoSyntaxElement>();
        while (current.Type is not (MiHoYoSyntaxTokenType.ItalicClose or MiHoYoSyntaxTokenType.EndOfFile))
        {
            children.Add(ParseElement());
        }

        if (current.Type is not MiHoYoSyntaxTokenType.ItalicClose)
        {
            throw HutaoException.Throw("Expected </i>");
        }

        int end = current.Position.End;
        NextToken();
        return new MiHoYoSyntaxItalicElement(new(start, end), children.ToImmutable());
    }

    private MiHoYoSyntaxElement ParseColor()
    {
        int start = current.Position.Start;
        int end = current.Position.End;
        NextToken();

        ImmutableArray<MiHoYoSyntaxElement>.Builder children = ImmutableArray.CreateBuilder<MiHoYoSyntaxElement>();
        while (current.Type is not (MiHoYoSyntaxTokenType.ColorClose or MiHoYoSyntaxTokenType.EndOfFile))
        {
            children.Add(ParseElement());
        }

        if (current.Type is not MiHoYoSyntaxTokenType.ColorClose)
        {
            throw HutaoException.Throw("Expected </color>");
        }

        int closeEnd = current.Position.End;
        NextToken();
        return new MiHoYoSyntaxColorElement(new(start, closeEnd), new(start + 8, end - 1), children.ToImmutable());
    }

    private MiHoYoSyntaxElement ParseLink()
    {
        int start = current.Position.Start;
        int end = current.Position.End;
        NextToken();

        ImmutableArray<MiHoYoSyntaxElement>.Builder children = ImmutableArray.CreateBuilder<MiHoYoSyntaxElement>();
        while (current.Type is not (MiHoYoSyntaxTokenType.LinkClose or MiHoYoSyntaxTokenType.EndOfFile))
        {
            children.Add(ParseElement());
        }

        if (current.Type is not MiHoYoSyntaxTokenType.LinkClose)
        {
            throw HutaoException.Throw("Expected {/LINK}");
        }

        int closeEnd = current.Position.End;
        NextToken();
        return new MiHoYoSyntaxLinkElement(new(start, closeEnd), new(start + 6, end - 1), children.ToImmutable());
    }
}