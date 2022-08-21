// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
// some part of this file came from:
// https://github.com/xunkong/desktop/tree/main/src/Desktop/Desktop/Pages/CharacterInfoPage.xaml.cs

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Core;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Windows.UI;

namespace Snap.Hutao.Control.Text;

/// <summary>
/// 专用于呈现描述文本的文本块
/// </summary>
public class DescriptionTextBlock : ContentControl
{
    private static readonly DependencyProperty DescriptionProperty =
        Property<DescriptionTextBlock>.Depend(nameof(Description), string.Empty, OnDescriptionChanged);

    private static readonly Regex ColorRegex = new(@"<color=([^>]+)>([^<]+)</color>", RegexOptions.Compiled);

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

        string[] lines = ((string)e.NewValue).Split('\n');

        foreach (string line in lines)
        {
            string left, right = line;

            foreach (Match match in ColorRegex.Matches(line))
            {
                string fullMatch = match.Groups[0].Value;
                int matchPosition = right.IndexOf(fullMatch);
                left = right[..matchPosition];
                right = right[(matchPosition + fullMatch.Length)..];

                if (!string.IsNullOrWhiteSpace(left))
                {
                    text.Inlines.Add(new Run { Text = left });
                }

                string hexColor = match.Groups[1].Value;
                string content = match.Groups[2].Value;

                text.Inlines.Add(new Run { Text = content, Foreground = new SolidColorBrush(new HexColor(hexColor[1..])) });
            }

            if (!string.IsNullOrWhiteSpace(right))
            {
                if (right.Contains("<i>"))
                {
                    string italic = right.Replace("<i>", string.Empty).Replace("</i>", string.Empty);
                    text.Inlines.Add(new Run { Text = italic, FontStyle = Windows.UI.Text.FontStyle.Italic });
                }
                else
                {
                    text.Inlines.Add(new Run { Text = right });
                }
            }

            text.Inlines.Add(new LineBreak());
        }

        if (text.Inlines.LastOrDefault() is LineBreak newline)
        {
            text.Inlines.Remove(newline);
        }
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

        public HexColor(string hex)
        {
            R = 0;
            G = 0;
            B = 0;
            A = 0;
            data = Convert.ToUInt32(hex, 16);
        }

        public static implicit operator Color(HexColor hexColor)
        {
            return Color.FromArgb(hexColor.A, hexColor.R, hexColor.G, hexColor.B);
        }
    }
}
