// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
// some part of this file came from:
// https://github.com/xunkong/desktop/tree/main/src/Desktop/Desktop/Pages/CharacterInfoPage.xaml.cs

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Core;
using Windows.UI;

namespace Snap.Hutao.Control.Text;

/// <summary>
/// 专用于呈现描述文本的文本块
/// </summary>
public class DescriptionTextBlock : ContentControl
{
    private static readonly DependencyProperty DescriptionProperty =
        Property<DescriptionTextBlock>.Depend(nameof(Description), string.Empty, OnDescriptionChanged);

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
        Content = new TextBlock();
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
        text.Inlines.Clear();

        ReadOnlySpan<char> description = (string)e.NewValue;

        int last = 0;
        for (int i = 0; i < description.Length;)
        {
            if (description[i] == '\\' && description[i + 1] == 'n')
            {
                AppendText(text, description[last..i]);
                AppendLineBreak(text);
                i += 1;
                last = i;
            }
            else if (description[i] == '<' && description[i + 1] == 'c')
            {
                AppendText(text, description[last..i]);

                byte[] data = Convert.FromHexString(description.Slice(i + 8, 8));
                Color color = Color.FromArgb(data[3], data[0], data[1], data[2]);

                int length = description[(i + ColorTagLeftLength)..].IndexOf('<');
                AppendColorText(text, description.Slice(i + ColorTagLeftLength, length), color);

                i += length + ColorTagFullLength;
                last = i;
            }
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
                AppendText(text, description[last..i]);
            }
        }
    }

    private static void AppendText(TextBlock text, ReadOnlySpan<char> slice)
    {
        text.Inlines.Add(new Run { Text = slice.ToString() });
    }

    private static void AppendColorText(TextBlock text, ReadOnlySpan<char> slice, Color color)
    {
        text.Inlines.Add(new Run
        {
            Text = slice.ToString(),
            Foreground = new SolidColorBrush(color),
        });
    }

    private static void AppendItalicText(TextBlock text, ReadOnlySpan<char> slice)
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
}
