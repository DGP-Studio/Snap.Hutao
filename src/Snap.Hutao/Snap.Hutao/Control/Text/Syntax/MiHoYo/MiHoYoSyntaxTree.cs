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
        ParseLines(text, root);

        MiHoYoSyntaxTree tree = new()
        {
            Text = text,
            Root = root,
        };

        return tree;
    }

    private static void ParseLines(string text, MiHoYoRootSyntax syntax)
    {
        ReadOnlySpan<char> textSpan = text.AsSpan();
        int previousProcessedIndexOfText = 0;

        while (true)
        {
            int newLineIndexAtSlicedText = textSpan[previousProcessedIndexOfText..].IndexOf('\n');

            if (newLineIndexAtSlicedText < 0)
            {
                MiHoYoLineSyntax line = new(false, text, previousProcessedIndexOfText, textSpan.Length);
                ParseComponents(text, line);
                syntax.Children.Add(line);
                break;
            }

            MiHoYoLineSyntax lineWithBreaking = new(true, text, previousProcessedIndexOfText, previousProcessedIndexOfText + newLineIndexAtSlicedText + 1);
            ParseComponents(text, lineWithBreaking);
            syntax.Children.Add(lineWithBreaking);

            previousProcessedIndexOfText = lineWithBreaking.Position.End;
        }
    }

    private static void ParseComponents(string text, MiHoYoSyntaxNode syntax)
    {
        TextPosition contentPosition = syntax switch
        {
            MiHoYoXmlElementSyntax xmlSyntax => xmlSyntax.ContentPosition,
            MiHoYoLineSyntax lineSyntax => lineSyntax.TextPosition,
            _ => syntax.Position,
        };
        ReadOnlySpan<char> contentSpan = text.AsSpan().Slice(contentPosition.Start, contentPosition.Length);

        int previousProcessedIndexOfContent = 0;
        while (true)
        {
            int fullXmlOpeningIndexOfContent = contentSpan[previousProcessedIndexOfContent..].IndexOf('<');

            // End of content
            if (fullXmlOpeningIndexOfContent < 0)
            {
                MiHoYoPlainTextSyntax plainText = new(text, contentPosition.Start + previousProcessedIndexOfContent, contentPosition.End);
                syntax.Children.Add(plainText);
                break;
            }

            // We have plain text between xml elements
            if (previousProcessedIndexOfContent < fullXmlOpeningIndexOfContent)
            {
                MiHoYoPlainTextSyntax plainText = new(text, contentPosition.Start + previousProcessedIndexOfContent, contentPosition.End);
                syntax.Children.Add(plainText);
            }

            // Peek the next character after '<'
            switch (contentSpan[previousProcessedIndexOfContent + fullXmlOpeningIndexOfContent + 1])
            {
                case 'c':
                    {
                        // <color=#FFFFFFFF></color>
                        // <color=#FFFFFF></color>
                        int colorTagClosingEndOfSlicedContent = IndexOfClosingEnd(contentSpan[fullXmlOpeningIndexOfContent..], out int colorTagLeftClosingEndOfSlicedContent);

                        MiHoYoColorKind colorKind = colorTagLeftClosingEndOfSlicedContent switch
                        {
                            17 => MiHoYoColorKind.Rgba,
                            15 => MiHoYoColorKind.Rgb,
                            _ => throw Must.NeverHappen(),
                        };

                        TextPosition positionOfColorElement = new(0, colorTagClosingEndOfSlicedContent);
                        TextPosition positionAtContent = positionOfColorElement.Add(fullXmlOpeningIndexOfContent);
                        TextPosition positionAtText = positionAtContent.Add(contentPosition.Start + previousProcessedIndexOfContent);

                        MiHoYoColorTextSyntax colorText = new(colorKind, text, positionAtText);
                        ParseComponents(text, colorText);
                        syntax.Children.Add(colorText);
                        previousProcessedIndexOfContent = positionAtContent.End;
                        break;
                    }

                case 'i':
                    {
                        // <i>sometext</i> 14
                        int italicTagClosingEndOfSlicedContent = IndexOfClosingEnd(contentSpan[fullXmlOpeningIndexOfContent..], out _);

                        TextPosition positionOfItalicElement = new(0, italicTagClosingEndOfSlicedContent);
                        TextPosition positionAtContent = positionOfItalicElement.Add(fullXmlOpeningIndexOfContent);
                        TextPosition positionAtText = positionAtContent.Add(contentPosition.Start + previousProcessedIndexOfContent);

                        MiHoYoItalicTextSyntax italicText = new(text, positionAtText);
                        ParseComponents(text, italicText);
                        syntax.Children.Add(italicText);
                        previousProcessedIndexOfContent = positionAtContent.End;
                        break;
                    }
            }
        }
    }

    private static int IndexOfClosingEnd(in ReadOnlySpan<char> span, out int leftClosingEnd)
    {
        leftClosingEnd = 0;

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
                leftClosingEnd = current;
            }

            if (openingCount == closingCount)
            {
                return current;
            }
        }
    }
}