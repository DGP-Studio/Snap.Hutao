// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;

namespace Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;

internal sealed class MiHoYoSyntaxTree
{
    public MiHoYoSyntaxNode Root { get; init; } = default!;

    public string Text { get; set; } = default!;

    public static MiHoYoSyntaxTree Parse(string text)
    {
        MiHoYoRootSyntax root = new(text, 0, text.Length);
        ParseComponents(text, root);

        MiHoYoSyntaxTree tree = new()
        {
            Text = text,
            Root = root,
        };

        return tree;
    }

    private static void ParseComponents(string text, MiHoYoSyntaxNode syntax)
    {
        TextPosition contentPosition = syntax switch
        {
            MiHoYoXmlLikeElementSyntax xmlLikeSyntax => xmlLikeSyntax.ContentPosition,
            _ => syntax.Position,
        };
        ReadOnlySpan<char> contentSpan = text.AsSpan().Slice(contentPosition.Start, contentPosition.Length);

        int endOfProcessedAtContent = 0;
        while (true)
        {
            if (endOfProcessedAtContent >= contentSpan.Length)
            {
                break;
            }

            int indexOfLeftOpeningAtUnprocessedContent = contentSpan[endOfProcessedAtContent..].IndexOfAny("<{");

            // End of content
            if (indexOfLeftOpeningAtUnprocessedContent < 0)
            {
                TextPosition position = new(contentPosition.Start + endOfProcessedAtContent, contentPosition.End);
                MiHoYoPlainTextSyntax plainText = new(text, position);
                syntax.Children.Add(plainText);
                break;
            }

            // We have plain text between elements
            if (indexOfLeftOpeningAtUnprocessedContent > 0)
            {
                TextPosition position = new(0, indexOfLeftOpeningAtUnprocessedContent);
                TextPosition positionAtContent = position.RightShift(endOfProcessedAtContent);
                TextPosition positionAtText = positionAtContent.RightShift(contentPosition.Start);
                MiHoYoPlainTextSyntax plainText = new(text, positionAtText);
                syntax.Children.Add(plainText);
                endOfProcessedAtContent = positionAtContent.End;
                continue;
            }

            // Peek the next character after '<' or '{'
            int indexOfLeftOpeningAtContent = endOfProcessedAtContent + indexOfLeftOpeningAtUnprocessedContent;
            switch (contentSpan[indexOfLeftOpeningAtContent + 1])
            {
                case 'c':
                    {
                        int endOfXmlColorRightClosingAtUnprocessedContent = EndOfXmlClosing(contentSpan[indexOfLeftOpeningAtContent..], out int endOfXmlColorLeftClosingAtUnprocessedContent);

                        MiHoYoColorKind colorKind = endOfXmlColorLeftClosingAtUnprocessedContent switch
                        {
                            17 => MiHoYoColorKind.Rgba,
                            15 => MiHoYoColorKind.Rgb,
                            _ => throw HutaoException.NotSupported(),
                        };

                        TextPosition position = new(0, endOfXmlColorRightClosingAtUnprocessedContent);
                        TextPosition positionAtContent = position.RightShift(endOfProcessedAtContent);
                        TextPosition positionAtText = positionAtContent.RightShift(contentPosition.Start);

                        MiHoYoColorTextSyntax colorText = new(colorKind, text, positionAtText);
                        ParseComponents(text, colorText);
                        syntax.Children.Add(colorText);
                        endOfProcessedAtContent = positionAtContent.End;
                        break;
                    }

                case 'i':
                    {
                        int endOfXmlItalicRightClosingAtUnprocessedContent = EndOfXmlClosing(contentSpan[indexOfLeftOpeningAtContent..], out _);

                        TextPosition position = new(0, endOfXmlItalicRightClosingAtUnprocessedContent);
                        TextPosition positionAtContent = position.RightShift(endOfProcessedAtContent);
                        TextPosition positionAtText = positionAtContent.RightShift(contentPosition.Start);

                        MiHoYoItalicTextSyntax italicText = new(text, positionAtText);
                        ParseComponents(text, italicText);
                        syntax.Children.Add(italicText);
                        endOfProcessedAtContent = positionAtContent.End;
                        break;
                    }

                case 'L':
                    {
                        int endOfLinkRightClosingAtUnprocessedContent = EndOfLinkClosing(contentSpan[indexOfLeftOpeningAtContent..], out int endOfLinkLeftClosingAtUnprocessedContent);

                        MiHoYoLinkKind linkKind = contentSpan[indexOfLeftOpeningAtContent..][6] switch
                        {
                            'P' => MiHoYoLinkKind.Inherent,
                            'N' => MiHoYoLinkKind.Name,
                            'S' => MiHoYoLinkKind.Skill,
                            _ => throw HutaoException.NotSupported(),
                        };

                        TextPosition position = new(0, endOfLinkRightClosingAtUnprocessedContent);
                        TextPosition positionAtContent = position.RightShift(endOfProcessedAtContent);
                        TextPosition positionAtText = positionAtContent.RightShift(contentPosition.Start);

                        MiHoYoLinkTextSyntax linkText = new(linkKind, endOfLinkLeftClosingAtUnprocessedContent - 8, text, positionAtText);
                        ParseComponents(text, linkText);
                        syntax.Children.Add(linkText);
                        endOfProcessedAtContent = positionAtContent.End;
                        break;
                    }
            }
        }
    }

    private static int EndOfXmlClosing(in ReadOnlySpan<char> span, out int endOfLeftClosing)
    {
        return EndOfClosing(span, '<', '>', out endOfLeftClosing);
    }

    private static int EndOfLinkClosing(in ReadOnlySpan<char> span, out int endOfLeftClosing)
    {
        return EndOfClosing(span, '{', '}', out endOfLeftClosing);
    }

    private static int EndOfClosing(in ReadOnlySpan<char> span, char leftMark, char rightMark, out int endOfLeftClosing)
    {
        endOfLeftClosing = 0;

        int openingCount = 0;
        int closingCount = 0;

        int current = 0;

        while (true)
        {
            int leftMarkIndex = span[current..].IndexOf(leftMark);
            if (span[current..][leftMarkIndex + 1] is '/')
            {
                closingCount++;
            }
            else
            {
                openingCount++;
            }

            current += span[current..].IndexOf(rightMark) + 1;

            if (openingCount is 1 && closingCount is 0)
            {
                endOfLeftClosing = current;
            }

            if (openingCount == closingCount)
            {
                return current;
            }
        }
    }
}