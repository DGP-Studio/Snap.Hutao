// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Control.Media;
using Snap.Hutao.Control.Text.Syntax.MiHoYo;
using Snap.Hutao.Control.Theme;
using Snap.Hutao.Metadata;
using Windows.Foundation;
using Windows.UI;

namespace Snap.Hutao.Control.Text;

/// <summary>
/// 专用于呈现描述文本的文本块
/// </summary>
[HighQuality]
[DependencyProperty("Description", typeof(string), "", nameof(OnDescriptionChanged))]
[DependencyProperty("TextStyle", typeof(Style), default(Style), nameof(OnTextStyleChanged))]
internal sealed partial class DescriptionTextBlock : ContentControl
{
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

        try
        {
            UpdateDescription(textBlock, MiHoYoSyntaxTree.Parse(SpecialNameHandler.Handle((string)e.NewValue)));
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

    private static void UpdateDescription(TextBlock textBlock, MiHoYoSyntaxTree syntaxTree)
    {
        textBlock.Inlines.Clear();
        AppendNode(textBlock, textBlock.Inlines, syntaxTree.Root);
    }

    private static void AppendNode(TextBlock textBlock, InlineCollection inlines, MiHoYoSyntaxNode node)
    {
        switch (node.Kind)
        {
            case MiHoYoSyntaxKind.Root:
                foreach (MiHoYoSyntaxNode child in ((MiHoYoRootSyntax)node).Children)
                {
                    AppendNode(textBlock, inlines, child);
                }

                break;
            case MiHoYoSyntaxKind.PlainText:
                AppendPlainText(textBlock, inlines, (MiHoYoPlainTextSyntax)node);
                break;
            case MiHoYoSyntaxKind.ColorText:
                AppendColorText(textBlock, inlines, (MiHoYoColorTextSyntax)node);
                break;
            case MiHoYoSyntaxKind.ItalicText:
                AppendItalicText(textBlock, inlines, (MiHoYoItalicTextSyntax)node);
                break;
        }
    }

    private static void AppendPlainText(TextBlock textBlock, InlineCollection inlines, MiHoYoPlainTextSyntax plainText)
    {
        // PlainText doesn't have children
        inlines.Add(new Run { Text = plainText.Span.ToString() });
    }

    private static void AppendColorText(TextBlock textBlock, InlineCollection inlines, MiHoYoColorTextSyntax colorText)
    {
        Rgba32 color = new(colorText.ColorSpan.ToString());
        Color targetColor;
        if (ThemeHelper.IsDarkMode(textBlock.ActualTheme))
        {
            targetColor = color;
        }
        else
        {
            // Make lighter in light mode
            Hsla32 hsl = color.ToHsl();
            hsl.L *= 0.3;
            targetColor = Rgba32.FromHsl(hsl);
        }

        if (colorText.Children.Count > 1)
        {
            Span span = new()
            {
                Foreground = new SolidColorBrush(targetColor),
            };

            foreach (MiHoYoSyntaxNode child in colorText.Children)
            {
                AppendNode(textBlock, span.Inlines, child);
            }
        }
        else
        {
            inlines.Add(new Run
            {
                Text = colorText.ContentSpan.ToString(),
                Foreground = new SolidColorBrush(targetColor),
            });
        }
    }

    private static void AppendItalicText(TextBlock textBlock, InlineCollection inlines, MiHoYoItalicTextSyntax italicText)
    {
        if (italicText.Children.Count > 1)
        {
            Span span = new()
            {
                FontStyle = Windows.UI.Text.FontStyle.Italic,
            };

            foreach (MiHoYoSyntaxNode child in italicText.Children)
            {
                AppendNode(textBlock, span.Inlines, child);
            }
        }
        else
        {
            inlines.Add(new Run
            {
                Text = italicText.ContentSpan.ToString(),
                FontStyle = Windows.UI.Text.FontStyle.Italic,
            });
        }
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        // Simply re-apply texts
        UpdateDescription((TextBlock)Content, MiHoYoSyntaxTree.Parse(SpecialNameHandler.Handle(Description)));
    }
}