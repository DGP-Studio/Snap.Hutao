// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Control.Media;
using Snap.Hutao.Control.Theme;
using Windows.Foundation;
using Windows.UI;

namespace Snap.Hutao.Control.Text;

[DependencyProperty("Description", typeof(string), "", nameof(OnDescriptionChanged))]
[DependencyProperty("TextStyle", typeof(Style), default(Style), nameof(OnTextStyleChanged))]
internal sealed partial class HtmlDescriptionTextBlock : ContentControl
{
    private static readonly int ColorTagFullLength = "<color style='color:#FFFFFF;'></color>".Length;
    private static readonly int ColorTagLeftLength = "<color style='color:#FFFFFF;'>".Length;

    private static readonly int ItalicTagFullLength = "<i></i>".Length;
    private static readonly int ItalicTagLeftLength = "<i>".Length;

    private static readonly int BoldTagFullLength = "<b></b>".Length;
    private static readonly int BoldTagLeftLength = "<b>".Length;

    private readonly TypedEventHandler<FrameworkElement, object> actualThemeChangedEventHandler;

    /// <summary>
    /// 构造一个新的呈现描述文本的文本块
    /// </summary>
    public HtmlDescriptionTextBlock()
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
        TextBlock textBlock = (TextBlock)((HtmlDescriptionTextBlock)d).Content;
        ReadOnlySpan<char> description = (string)e.NewValue;

        UpdateDescription(textBlock, description);
    }

    private static void OnTextStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        TextBlock textBlock = (TextBlock)((HtmlDescriptionTextBlock)d).Content;
        textBlock.Style = (Style)e.NewValue;
    }

    private static void UpdateDescription(TextBlock textBlock, in ReadOnlySpan<char> description)
    {
        textBlock.Inlines.Clear();

        int last = 0;
        for (int i = 0; i < description.Length;)
        {
            // newline
            if (description[i..].StartsWith(@"<br>"))
            {
                AppendText(textBlock, description[last..i]);
                AppendLineBreak(textBlock);
                i += 4;
                last = i;
            }

            // color tag
            else if (description[i..].StartsWith("<c"))
            {
                AppendText(textBlock, description[last..i]);
                string a = description.Slice(i + 21, 6).ToString();
                Rgba32 color = new(a);
                int length = description[(i + ColorTagLeftLength)..].IndexOf('<');
                AppendColorText(textBlock, description.Slice(i + ColorTagLeftLength, length), color);

                i += length + ColorTagFullLength;
                last = i;
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

            // bold
            else if (description[i..].StartsWith("<b"))
            {
                AppendText(textBlock, description[last..i]);

                int length = description[(i + BoldTagLeftLength)..].IndexOf('<');
                AppendBoldText(textBlock, description.Slice(i + BoldTagLeftLength, length));

                i += length + BoldTagFullLength;
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

    private static void AppendBoldText(TextBlock text, in ReadOnlySpan<char> slice)
    {
        text.Inlines.Add(new Run
        {
            Text = slice.ToString(),
            FontWeight = FontWeights.Bold,
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