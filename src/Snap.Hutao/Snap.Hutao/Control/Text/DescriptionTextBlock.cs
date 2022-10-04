// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
// Some part of this file came from:
// https://github.com/xunkong/desktop/tree/main/src/Desktop/Desktop/Pages/CharacterInfoPage.xaml.cs

using CommunityToolkit.WinUI;
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

    private static void ApplyDescription(TextBlock text, ReadOnlySpan<char> description)
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
                HexColor color = new(description.Slice(i + 8, 8));
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

    private static void AppendText(TextBlock text, ReadOnlySpan<char> slice)
    {
        text.Inlines.Add(new Run { Text = slice.ToString() });
    }

    private static void AppendColorText(TextBlock text, ReadOnlySpan<char> slice, HexColor color)
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
            targetColor = HexColor.FromHsl(hsl);
        }

        text.Inlines.Add(new Run
        {
            Text = slice.ToString(),
            Foreground = new SolidColorBrush(targetColor),
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

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        ApplyDescription((TextBlock)Content, Description);
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

        private HexColor(byte r, byte g, byte b, byte a)
        {
            data = 0;
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static implicit operator Color(HexColor hexColor)
        {
            return Color.FromArgb(hexColor.A, hexColor.R, hexColor.G, hexColor.B);
        }

        public static HexColor FromHsl(HslColor hsl)
        {
            double chroma = (1 - Math.Abs((2 * hsl.L) - 1)) * hsl.S;
            double h1 = hsl.H / 60;
            double x = chroma * (1 - Math.Abs((h1 % 2) - 1));
            double m = hsl.L - (0.5 * chroma);
            double r1, g1, b1;

            if (h1 < 1)
            {
                r1 = chroma;
                g1 = x;
                b1 = 0;
            }
            else if (h1 < 2)
            {
                r1 = x;
                g1 = chroma;
                b1 = 0;
            }
            else if (h1 < 3)
            {
                r1 = 0;
                g1 = chroma;
                b1 = x;
            }
            else if (h1 < 4)
            {
                r1 = 0;
                g1 = x;
                b1 = chroma;
            }
            else if (h1 < 5)
            {
                r1 = x;
                g1 = 0;
                b1 = chroma;
            }
            else
            {
                r1 = chroma;
                g1 = 0;
                b1 = x;
            }

            byte r = (byte)(255 * (r1 + m));
            byte g = (byte)(255 * (g1 + m));
            byte b = (byte)(255 * (b1 + m));
            byte a = (byte)(255 * hsl.A);

            return new(r, g, b, a);
        }

        public HslColor ToHsl()
        {
            const double toDouble = 1.0 / 255;
            double r = toDouble * R;
            double g = toDouble * G;
            double b = toDouble * B;
            double max = Math.Max(Math.Max(r, g), b);
            double min = Math.Min(Math.Min(r, g), b);
            double chroma = max - min;
            double h1;

            if (chroma == 0)
            {
                h1 = 0;
            }
            else if (max == r)
            {
                // The % operator doesn't do proper modulo on negative
                // numbers, so we'll add 6 before using it
                h1 = (((g - b) / chroma) + 6) % 6;
            }
            else if (max == g)
            {
                h1 = 2 + ((b - r) / chroma);
            }
            else
            {
                h1 = 4 + ((r - g) / chroma);
            }

            double lightness = 0.5 * (max + min);
            double saturation = chroma == 0 ? 0 : chroma / (1 - Math.Abs((2 * lightness) - 1));

            HslColor ret;
            ret.H = 60 * h1;
            ret.S = saturation;
            ret.L = lightness;
            ret.A = toDouble * A;
            return ret;
        }
    }
}
