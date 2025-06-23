// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.ViewModel.Wiki;
using System.Collections.Immutable;
using System.Globalization;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using MUXCTextBlock = Microsoft.UI.Xaml.Controls.TextBlock;

namespace Snap.Hutao.UI.Xaml.Control.TextBlock;

[DependencyProperty("Description", typeof(string), "", nameof(OnDescriptionChanged))]
[DependencyProperty("LinkContext", typeof(LinkMetadataContext), default(LinkMetadataContext))]
[DependencyProperty("TextStyle", typeof(Style), default(Style), nameof(OnTextStyleChanged))]
internal sealed partial class DescriptionTextBlock : ContentControl
{
    public DescriptionTextBlock()
    {
        Content = new MUXCTextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            Style = TextStyle,
        };

        ActualThemeChanged += OnActualThemeChanged;
    }

    private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        DescriptionTextBlock descriptionTextBlock = (DescriptionTextBlock)d;
        MUXCTextBlock textBlock = (MUXCTextBlock)descriptionTextBlock.Content;
        descriptionTextBlock.UpdateDescription(textBlock, (string)e.NewValue);
    }

    private static void OnTextStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((MUXCTextBlock)((DescriptionTextBlock)d).Content).Style = (Style)e.NewValue;
    }

    private void UpdateDescription(MUXCTextBlock textBlock, string text)
    {
        DetachHyperLinkClickEvent(textBlock.Inlines);
        textBlock.Inlines.Clear();

        string content = SpecialNameHandling.Handle(text);
        MiHoYoSyntaxLexer lexer = new(content);
        MiHoYoSyntaxParser parser = new(lexer);
        ImmutableArray<MiHoYoSyntaxElement> elements = parser.Parse();
        string format = MiHoYoSyntaxFormatter.Format(elements, content);

        foreach (ref readonly MiHoYoSyntaxElement element in elements.AsSpan())
        {
            AppendNode(textBlock, textBlock.Inlines, element, content);
        }
    }

    private void DetachHyperLinkClickEvent(InlineCollection inlineCollection)
    {
        foreach (Inline inline in inlineCollection)
        {
            if (inline is Span span)
            {
                DetachHyperLinkClickEvent(span.Inlines);
            }

            if (inline is Hyperlink link)
            {
                // Unsubscribe from the click event
                link.Click -= OnLinkClicked;
            }
        }
    }

    private void AppendNode(MUXCTextBlock textBlock, InlineCollection inlines, MiHoYoSyntaxElement syntaxElement, ReadOnlySpan<char> content)
    {
        _ = syntaxElement switch
        {
            MiHoYoSyntaxTextElement textElement => AppendPlainText(textBlock, inlines, textElement, content),
            MiHoYoSyntaxColorElement colorElement => AppendColorText(textBlock, inlines, colorElement, content),
            MiHoYoSyntaxItalicElement italicElement => AppendItalicText(textBlock, inlines, italicElement, content),
            MiHoYoSyntaxLinkElement linkElement => AppendLinkText(textBlock, inlines, linkElement, content),
            _ => default,
        };
    }

    [SuppressMessage("", "CA1822")]
    private Void AppendPlainText(MUXCTextBlock textBlock, InlineCollection inlines, MiHoYoSyntaxTextElement syntaxTextElement, ReadOnlySpan<char> content)
    {
        // PlainText doesn't have children
        inlines.Add(new Run { Text = syntaxTextElement.GetSpan(content).ToString() });
        return default;
    }

    private Void AppendColorText(MUXCTextBlock textBlock, InlineCollection inlines, MiHoYoSyntaxColorElement syntaxColorText, ReadOnlySpan<char> content)
    {
        if (syntaxColorText.Children.Length <= 0)
        {
            return default;
        }

        Rgba32 color = new(syntaxColorText.GetColorSpan(content).ToString());
        Color targetColor;
        if (ThemeHelper.IsDarkMode(textBlock.ActualTheme))
        {
            targetColor = color;
        }
        else
        {
            // Make lighter in light mode
            Hsla256 hsl = ColorHelper.ToHsla32(color);
            hsl.L *= 0.3;
            targetColor = ColorHelper.ToRgba32(hsl);
        }

        Span span = new()
        {
            Foreground = new SolidColorBrush(targetColor),
        };

        foreach (MiHoYoSyntaxElement child in syntaxColorText.Children)
        {
            AppendNode(textBlock, span.Inlines, child, content);
        }

        inlines.Add(span);

        return default;
    }

    private Void AppendItalicText(MUXCTextBlock textBlock, InlineCollection inlines, MiHoYoSyntaxItalicElement syntaxItalicText, ReadOnlySpan<char> content)
    {
        if (syntaxItalicText.Children.Length <= 0)
        {
            return default;
        }

        Span span = new()
        {
            FontStyle = FontStyle.Italic,
        };

        foreach (MiHoYoSyntaxElement child in syntaxItalicText.Children)
        {
            AppendNode(textBlock, span.Inlines, child, content);
        }

        inlines.Add(span);

        return default;
    }

    private Void AppendLinkText(MUXCTextBlock textBlock, InlineCollection inlines, MiHoYoSyntaxLinkElement syntaxLinkText, ReadOnlySpan<char> content)
    {
        if (syntaxLinkText.Children.Length <= 0)
        {
            return default;
        }

        Hyperlink span = new();
        DescriptionHyperLinkHelper.SetLinkData(span, Tuple.Create(syntaxLinkText.GetLinkKind(content), uint.Parse(syntaxLinkText.GetIdSpan(content)[1..], CultureInfo.InvariantCulture)));
        span.Click += OnLinkClicked;

        foreach (MiHoYoSyntaxElement child in syntaxLinkText.Children)
        {
            AppendNode(textBlock, span.Inlines, child, content);
        }

        inlines.Add(span);
        return default;
    }

    private void OnLinkClicked(Hyperlink sender, HyperlinkClickEventArgs args)
    {
        if (LinkContext is null)
        {
            return;
        }

        (MiHoYoSyntaxLinkKind kind, uint id) = DescriptionHyperLinkHelper.GetLinkData(sender);

        LinkContext.TryGetNameAndDescription(kind, id, out string name, out string description);

        Flyout flyout = new()
        {
            ShouldConstrainToRootBounds = false,
            Content = new LinkPresenter
            {
                LinkName = name,
                LinkDescription = description,
            },
        };

        Rect rect = sender.ElementStart.GetCharacterRect(LogicalDirection.Forward);
        Rect rectEnd = sender.ElementEnd.GetCharacterRect(LogicalDirection.Backward);

        // WPF Epsilon
        if (Math.Abs(rectEnd.Y - rect.Y) < 0.00000153)
        {
            rect.Width = rectEnd.X - rect.X;
        }

        rect = TransformToVisual(XamlRoot.Content).TransformBounds(rect);
        rect.X -= 8;
        rect.Y -= 8;
        rect.Width += 16;
        rect.Height += 16;

        FlyoutShowOptions options = new()
        {
            Position = new(rect.Left + (rect.Width / 2), rect.Y + (rect.Height / 2)),
            ExclusionRect = rect,
        };

        flyout.ShowAt(sender, options);
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        // Simply re-apply texts
        UpdateDescription((MUXCTextBlock)Content, Description);
    }
}