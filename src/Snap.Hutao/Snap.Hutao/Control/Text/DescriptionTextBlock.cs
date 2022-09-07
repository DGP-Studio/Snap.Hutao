// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
// some part of this file came from:
// https://github.com/xunkong/desktop/tree/main/src/Desktop/Desktop/Pages/CharacterInfoPage.xaml.cs

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Core;
using System.Runtime.InteropServices;
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
                HexColor color = new(description.Slice(i + 8, 8));
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

    private static void AppendColorText(TextBlock text, ReadOnlySpan<char> slice, HexColor color)
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

    [StructLayout(LayoutKind.Explicit)]
    private struct HexColor
    {
        [FieldOffset(3)]
        public byte R;
        [FieldOffset(2)]
        public byte G;
        [FieldOffset(1)]
        public byte B;
        [FieldOffset(0)]
        public byte A;

        [FieldOffset(0)]
        private readonly uint data;

        public HexColor(ReadOnlySpan<char> hex)
        {
            Must.Argument(hex.Length == 8, "色值长度不为8");
            R = 0;
            G = 0;
            B = 0;
            A = 0;
            data = Convert.ToUInt32(hex.ToString(), 16);
        }

        public static implicit operator Color(HexColor hexColor)
        {
            return Color.FromArgb(hexColor.A, hexColor.R, hexColor.G, hexColor.B);
        }
    }
}
