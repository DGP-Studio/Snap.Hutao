// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Control.Text.Syntax.MiHoYo;

internal sealed class MiHoYoSyntaxTree
{
    public MiHoYoSyntaxNode Root { get; set; } = default!;

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
            MiHoYoXmlElementSyntax xmlSyntax => xmlSyntax.ContentPosition,
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

            int indexOfXmlLeftOpeningAtUnprocessedContent = contentSpan[endOfProcessedAtContent..].IndexOf('<');

            // End of content
            if (indexOfXmlLeftOpeningAtUnprocessedContent < 0)
            {
                TextPosition position = new(contentPosition.Start + endOfProcessedAtContent, contentPosition.End);
                MiHoYoPlainTextSyntax plainText = new(text, position);
                syntax.Children.Add(plainText);
                break;
            }

            // We have plain text between xml elements
            if (indexOfXmlLeftOpeningAtUnprocessedContent > 0)
            {
                TextPosition position = new(0, indexOfXmlLeftOpeningAtUnprocessedContent);
                TextPosition positionAtContent = position.RightShift(endOfProcessedAtContent);
                TextPosition positionAtText = positionAtContent.RightShift(contentPosition.Start);
                MiHoYoPlainTextSyntax plainText = new(text, positionAtText);
                syntax.Children.Add(plainText);
                endOfProcessedAtContent = positionAtContent.End;
                continue;
            }

            // Peek the next character after '<'
            int indexOfXmlLeftOpeningAtContent = endOfProcessedAtContent + indexOfXmlLeftOpeningAtUnprocessedContent;
            switch (contentSpan[indexOfXmlLeftOpeningAtContent + 1])
            {
                case 'c':
                    {
                        int endOfXmlColorRightClosingAtUnprocessedContent = EndOfXmlClosing(contentSpan[indexOfXmlLeftOpeningAtContent..], out int endOfXmlColorLeftClosingAtUnprocessedContent);

                        MiHoYoColorKind colorKind = endOfXmlColorLeftClosingAtUnprocessedContent switch
                        {
                            17 => MiHoYoColorKind.Rgba,
                            15 => MiHoYoColorKind.Rgb,
                            _ => throw Must.NeverHappen(),
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
                        int endOfXmlItalicRightClosingAtUnprocessedContent = EndOfXmlClosing(contentSpan[indexOfXmlLeftOpeningAtContent..], out _);

                        TextPosition position = new(0, endOfXmlItalicRightClosingAtUnprocessedContent);
                        TextPosition positionAtContent = position.RightShift(endOfProcessedAtContent);
                        TextPosition positionAtText = positionAtContent.RightShift(contentPosition.Start);

                        MiHoYoItalicTextSyntax italicText = new(text, positionAtText);
                        ParseComponents(text, italicText);
                        syntax.Children.Add(italicText);
                        endOfProcessedAtContent = positionAtContent.End;
                        break;
                    }
            }
        }
    }

    private static int EndOfXmlClosing(in ReadOnlySpan<char> span, out int endOfLeftClosing)
    {
        endOfLeftClosing = 0;

        int openingCount = 0;
        int closingCount = 0;

        int current = 0;

        // Considering <i>text1</i>text2<i>text3</i>
        // Considering <i>text1<span>text2</span>text3</i>
        while (true)
        {
            int leftMarkIndex = span[current..].IndexOf('<');
            if (span[current..][leftMarkIndex + 1] is '/')
            {
                closingCount++;
            }
            else
            {
                openingCount++;
            }

            current += span[current..].IndexOf('>') + 1;

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