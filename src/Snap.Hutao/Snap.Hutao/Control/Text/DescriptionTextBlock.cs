// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Control.Media;
using Snap.Hutao.Control.Theme;
using Windows.UI;

namespace Snap.Hutao.Control.Text;

/// <summary>
/// 专用于呈现描述文本的文本块
/// Some part of this file came from:
/// https://github.com/xunkong/desktop/tree/main/src/Desktop/Desktop/Pages/CharacterInfoPage.xaml.cs
/// </summary>
[HighQuality]
internal sealed class DescriptionTextBlock : ContentControl
{
    private static readonly DependencyProperty DescriptionProperty = Property<DescriptionTextBlock>.Depend(nameof(Description), string.Empty, OnDescriptionChanged);

    private static readonly int ColorTagFullLength = "<color=#FFFFFFFF></color>".Length;
    private static readonly int ColorTagLeftLength = "<color=#FFFFFFFF>".Length;

    private static readonly int ItalicTagFullLength = "<i></i>".Length;
    private static readonly int ItalicTagLeftLength = "<i>".Length;

    /// <summary>
    /// 构造一个新的呈现描述文本的文本块
    /// </summary>
    public DescriptionTextBlock()
    {
        IsTabStop = false;

        Content = new TextBlock()
        {
            TextWrapping = TextWrapping.Wrap,
        };

        ActualThemeChanged += OnActualThemeChanged;
    }

    /// <summary>
    /// 可绑定的描述文本
    /// </summary>
    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        TextBlock text = (TextBlock)((DescriptionTextBlock)d).Content;
        ReadOnlySpan<char> description = (string)e.NewValue;

        ApplyDescription(text, description);
    }

    private static void ApplyDescription(TextBlock text, in ReadOnlySpan<char> description)
    {
        text.Inlines.Clear();

        int last = 0;
        for (int i = 0; i < description.Length;)
        {
            // newline
            if (description[i] == '\\' && description[i + 1] == 'n')
            {
                AppendText(text, description[last..i]);
                AppendLineBreak(text);
                i += 1;
                last = i;
            }

            // color tag
            else if (description[i] == '<' && description[i + 1] == 'c')
            {
                AppendText(text, description[last..i]);
                Rgba8 color = new(description.Slice(i + 8, 8));
                int length = description[(i + ColorTagLeftLength)..].IndexOf('<');
                AppendColorText(text, description.Slice(i + ColorTagLeftLength, length), color);

                i += length + ColorTagFullLength;
                last = i;
            }

            // italic
            else if (description[i] == '<' && description[i + 1] == 'i')
            {
                AppendText(text, description[last..i]);

                int length = description[(i + ItalicTagLeftLength)..].IndexOf('<');
                AppendItalicText(text, description.Slice(i + ItalicTagLeftLength, length));

                i += length + ItalicTagFullLength;
                last = i;
            }
            else
            {
                i += 1;
            }

            if (i == description.Length - 1)
            {
                AppendText(text, description[last..(i + 1)]);
            }
        }
    }

    private static void AppendText(TextBlock text, in ReadOnlySpan<char> slice)
    {
        text.Inlines.Add(new Run { Text = slice.ToString() });
    }

    private static void AppendColorText(TextBlock text, in ReadOnlySpan<char> slice, Rgba8 color)
    {
        Color targetColor;
        if (ThemeHelper.IsDarkMode(text.ActualTheme))
        {
            targetColor = color;
        }
        else
        {
            HslColor hsl = color.ToHsl();
            hsl.L *= 0.3;
            targetColor = Rgba8.FromHsl(hsl);
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
        ApplyDescription((TextBlock)Content, Description);
    }
}