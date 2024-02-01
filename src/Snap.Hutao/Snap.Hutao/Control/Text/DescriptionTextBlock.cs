// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Control.Media;
using Snap.Hutao.Control.Theme;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI;

namespace Snap.Hutao.Control.Text;

/// <summary>
/// 专用于呈现描述文本的文本块
/// Some part of this file came from:
/// https://github.com/xunkong/desktop/tree/main/src/Desktop/Desktop/Pages/CharacterInfoPage.xaml.cs
/// </summary>
[HighQuality]
[DependencyProperty("Description", typeof(string), "", nameof(OnDescriptionChanged))]
[DependencyProperty("TextStyle", typeof(Style), default(Style), nameof(OnTextStyleChanged))]
internal sealed partial class DescriptionTextBlock : ContentControl
{
    private static readonly int RgbaColorTagFullLength = "<color=#FFFFFFFF></color>".Length;
    private static readonly int RgbaColorTagLeftLength = "<color=#FFFFFFFF>".Length;

    private static readonly int RgbColorTagFullLength = "<color=#FFFFFF></color>".Length;
    private static readonly int RgbColorTagLeftLength = "<color=#FFFFFF>".Length;

    private static readonly int ItalicTagFullLength = "<i></i>".Length;
    private static readonly int ItalicTagLeftLength = "<i>".Length;

    private readonly TypedEventHandler<FrameworkElement, object> actualThemeChangedEventHandler;

    /// <summary>
    /// 构造一个新的呈现描述文本的文本块
    /// </summary>
    public DescriptionTextBlock()
    {
        this.DisableInteraction();

        Content = new TextBlock()
        {
            TextWrapping = TextWrapping.Wrap,
            Style = TextStyle,
        };

        actualThemeChangedEventHandler = OnActualThemeChanged;
        ActualThemeChanged += actualThemeChangedEventHandler;
    }

    private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        TextBlock textBlock = (TextBlock)((DescriptionTextBlock)d).Content;
        ReadOnlySpan<char> description = MetadataSpecialNames.Handle((string)e.NewValue);

        try
        {
            UpdateDescription(textBlock, description);
        }
        catch (Exception ex)
        {
            _ = ex;
        }
    }

    private static void OnTextStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        TextBlock textBlock = (TextBlock)((DescriptionTextBlock)d).Content;
        textBlock.Style = (Style)e.NewValue;
    }

    private static void UpdateDescription(TextBlock textBlock, in ReadOnlySpan<char> description)
    {
        textBlock.Inlines.Clear();

        int last = 0;
        for (int i = 0; i < description.Length;)
        {
            // newline
            if (description[i..].StartsWith(@"\n"))
            {
                AppendText(textBlock, description[last..i]);
                AppendLineBreak(textBlock);
                i += 2;
                last = i;
            }

            // color tag
            else if (description[i..].StartsWith("<c"))
            {
                switch (description[i..].IndexOf('>'))
                {
                    case 16: // RgbaColorTag
                        {
                            AppendText(textBlock, description[last..i]);
                            Rgba32 color = new(description.Slice(i + 8, 8).ToString());
                            int length = description[(i + RgbaColorTagLeftLength)..].IndexOf('<');
                            AppendColorText(textBlock, description.Slice(i + RgbaColorTagLeftLength, length), color);

                            i += length + RgbaColorTagFullLength;
                            last = i;
                            break;
                        }

                    case 14: // RgbColorTag
                        {
                            AppendText(textBlock, description[last..i]);
                            Rgba32 color = new(description.Slice(i + 8, 6).ToString());
                            int length = description[(i + RgbColorTagLeftLength)..].IndexOf('<');
                            AppendColorText(textBlock, description.Slice(i + RgbColorTagLeftLength, length), color);

                            i += length + RgbColorTagFullLength;
                            last = i;
                            break;
                        }
                }
            }

            // italic
            else if (description[i..].StartsWith("<i"))
            {
                AppendText(textBlock, description[last..i]);

                int length = description[(i + ItalicTagLeftLength)..].IndexOf('<');
                AppendItalicText(textBlock, description.Slice(i + ItalicTagLeftLength, length));

                i += length + ItalicTagFullLength;
                last = i;
            }
            else
            {
                i += 1;
            }

            if (i == description.Length - 1)
            {
                AppendText(textBlock, description[last..(i + 1)]);
            }
        }
    }

    private static void AppendText(TextBlock text, in ReadOnlySpan<char> slice)
    {
        text.Inlines.Add(new Run { Text = slice.ToString() });
    }

    private static void AppendColorText(TextBlock text, in ReadOnlySpan<char> slice, Rgba32 color)
    {
        Color targetColor;
        if (ThemeHelper.IsDarkMode(text.ActualTheme))
        {
            targetColor = color;
        }
        else
        {
            // Make lighter in light mode
            Hsl32 hsl = color.ToHsl();
            hsl.L *= 0.3;
            targetColor = Rgba32.FromHsl(hsl);
        }

        text.Inlines.Add(new Run
        {
            Text = slice.ToString(),
            Foreground = new SolidColorBrush(targetColor),
        });
    }

    private static void AppendItalicText(TextBlock text, in ReadOnlySpan<char> slice)
    {
        text.Inlines.Add(new Run
        {
            Text = slice.ToString(),
            FontStyle = Windows.UI.Text.FontStyle.Italic,
        });
    }

    private static void AppendLineBreak(TextBlock text)
    {
        text.Inlines.Add(new LineBreak());
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        // Simply re-apply texts
        UpdateDescription((TextBlock)Content, Description);
    }
}