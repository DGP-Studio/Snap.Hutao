// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.UI.Xaml.Control.TextBlock.Syntax.MiHoYo;
using Snap.Hutao.UI.Xaml.Control.Theme;
using Snap.Hutao.ViewModel.Wiki;
using System.Collections.Immutable;
using System.Globalization;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using WinRT;
using TextBlockType = Microsoft.UI.Xaml.Controls.RichTextBlock;

namespace Snap.Hutao.UI.Xaml.Control.TextBlock;

[DependencyProperty<string>("Description", DefaultValue = "", PropertyChangedCallbackName = nameof(OnDescriptionChanged))]
[DependencyProperty<LinkMetadataContext>("LinkContext")]
[DependencyProperty<Style>("TextStyle", PropertyChangedCallbackName = nameof(OnTextStyleChanged))]
internal sealed partial class DescriptionRichTextBlock : ContentControl
{
    private static readonly BitmapImage BitmapSourceIce = new("ms-appx:///Resource/Icon/UI_Gcg_Buff_Common_Element_Ice.png".ToUri());
    private static readonly BitmapImage BitmapSourceWater = new("ms-appx:///Resource/Icon/UI_Gcg_Buff_Common_Element_Water.png".ToUri());
    private static readonly BitmapImage BitmapSourceFire = new("ms-appx:///Resource/Icon/UI_Gcg_Buff_Common_Element_Fire.png".ToUri());
    private static readonly BitmapImage BitmapSourceElectric = new("ms-appx:///Resource/Icon/UI_Gcg_Buff_Common_Element_Electric.png".ToUri());
    private static readonly BitmapImage BitmapSourceWind = new("ms-appx:///Resource/Icon/UI_Gcg_Buff_Common_Element_Wind.png".ToUri());
    private static readonly BitmapImage BitmapSourceRock = new("ms-appx:///Resource/Icon/UI_Gcg_Buff_Common_Element_Rock.png".ToUri());
    private static readonly BitmapImage BitmapSourceGrass = new("ms-appx:///Resource/Icon/UI_Gcg_Buff_Common_Element_Grass.png".ToUri());

    public DescriptionRichTextBlock()
    {
        TextBlockType textBlock = new()
        {
            TextWrapping = TextWrapping.Wrap,
            Style = TextStyle,
        };
        textBlock.Blocks.Add(new Paragraph());
        Content = textBlock;

        ActualThemeChanged += OnActualThemeChanged;
    }

    private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        DescriptionRichTextBlock descriptionTextBlock = d.As<DescriptionRichTextBlock>();
        TextBlockType textBlock = descriptionTextBlock.Content.As<TextBlockType>();
        descriptionTextBlock.UpdateDescription(textBlock, e.NewValue.As<string>());
    }

    private static void OnTextStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        d.As<DescriptionRichTextBlock>().Content.As<TextBlockType>().Style = e.NewValue.As<Style>();
    }

    private void UpdateDescription(TextBlockType textBlock, string? text)
    {
        if (textBlock.Blocks is not [Paragraph paragraph])
        {
            return;
        }

        DetachHyperLinkClickEvent(paragraph.Inlines);
        paragraph.Inlines.Clear();

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        string content = SpecialNameHandling.Handle(text);
        MiHoYoSyntaxLexer lexer = new(content);
        MiHoYoSyntaxParser parser = new(lexer);
        ImmutableArray<MiHoYoSyntaxElement> elements = parser.Parse();
        foreach (ref readonly MiHoYoSyntaxElement element in elements.AsSpan())
        {
            AppendNode(textBlock, paragraph.Inlines, element, content);
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

    private void AppendNode(TextBlockType textBlock, InlineCollection inlines, MiHoYoSyntaxElement syntaxElement, ReadOnlySpan<char> source)
    {
        _ = syntaxElement switch
        {
            MiHoYoSyntaxTextElement textElement => AppendPlainText(textBlock, inlines, textElement, source),
            MiHoYoSyntaxColorElement colorElement => AppendColorText(textBlock, inlines, colorElement, source),
            MiHoYoSyntaxItalicElement italicElement => AppendItalicText(textBlock, inlines, italicElement, source),
            MiHoYoSyntaxLinkElement linkElement => AppendLinkText(textBlock, inlines, linkElement, source),
            MiHoYoSyntaxSpritePresetElement spritePresetElement => AppendSpritePreset(textBlock, inlines, spritePresetElement, source),
            _ => default,
        };
    }

    [SuppressMessage("", "CA1822")]
    private Void AppendPlainText(TextBlockType textBlock, InlineCollection inlines, MiHoYoSyntaxTextElement syntaxTextElement, ReadOnlySpan<char> source)
    {
        // PlainText doesn't have children
        inlines.Add(new Run { Text = syntaxTextElement.GetSpan(source).ToString() });
        return default;
    }

    private Void AppendColorText(TextBlockType textBlock, InlineCollection inlines, MiHoYoSyntaxColorElement syntaxColorText, ReadOnlySpan<char> source)
    {
        if (syntaxColorText.Children.Length <= 0)
        {
            return default;
        }

        Rgba32 color = new(syntaxColorText.GetColorSpan(source).ToString());
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
            AppendNode(textBlock, span.Inlines, child, source);
        }

        inlines.Add(span);

        return default;
    }

    private Void AppendItalicText(TextBlockType textBlock, InlineCollection inlines, MiHoYoSyntaxItalicElement syntaxItalicText, ReadOnlySpan<char> source)
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
            AppendNode(textBlock, span.Inlines, child, source);
        }

        inlines.Add(span);

        return default;
    }

    private Void AppendLinkText(TextBlockType textBlock, InlineCollection inlines, MiHoYoSyntaxLinkElement syntaxLinkText, ReadOnlySpan<char> source)
    {
        if (syntaxLinkText.Children.Length <= 0)
        {
            return default;
        }

        Hyperlink span = new();
        DescriptionHyperLinkHelper.SetLinkData(span, Tuple.Create(syntaxLinkText.GetLinkKind(source), uint.Parse(syntaxLinkText.GetIdSpan(source)[1..], CultureInfo.InvariantCulture)));
        span.Click += OnLinkClicked;

        foreach (MiHoYoSyntaxElement child in syntaxLinkText.Children)
        {
            AppendNode(textBlock, span.Inlines, child, source);
        }

        inlines.Add(span);
        return default;
    }

    private Void AppendSpritePreset(TextBlockType textBlock, InlineCollection inlines, MiHoYoSyntaxSpritePresetElement syntaxSpritePreset, ReadOnlySpan<char> source)
    {
        BitmapImage? imageSource = uint.Parse(syntaxSpritePreset.GetIdSpan(source), CultureInfo.InvariantCulture) switch
        {
            11001U => BitmapSourceIce,
            11002U => BitmapSourceWater,
            11003U => BitmapSourceFire,
            11004U => BitmapSourceElectric,
            11005U => BitmapSourceWind,
            11006U => BitmapSourceRock,
            11007U => BitmapSourceGrass,
            _ => default,
        };

        double size = textBlock.FontSize; // 1.2 is a magic number to make the icon size look good

        Microsoft.UI.Xaml.Controls.Image image = new()
        {
            RenderTransform = new CompositeTransform() { TranslateY = 2 },
            Width = size,
            Height = size,
            Source = imageSource,
        };

        InlineUIContainer container = new()
        {
            Child = image,
        };

        inlines.Add(container);

        return default;
    }

    private void OnLinkClicked(Hyperlink sender, HyperlinkClickEventArgs args)
    {
        if (LinkContext is null)
        {
            return;
        }

        Tuple<MiHoYoSyntaxLinkKind, uint>? tuple = DescriptionHyperLinkHelper.GetLinkData(sender);
        if (tuple is null)
        {
            return;
        }

        (MiHoYoSyntaxLinkKind kind, uint id) = tuple;

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
        UpdateDescription(Content.As<TextBlockType>(), Description);
    }
}