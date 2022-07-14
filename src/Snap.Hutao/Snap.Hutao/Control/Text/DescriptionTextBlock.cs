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

    /// <summary>
    /// 构造一个新的呈现描述文本的文本块
    /// </summary>
    public DescriptionTextBlock()
    {
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
            MatchCollection matches = Regex.Matches(line, @"<color=([^>]+)>([^<]+)</color>");
            string left, right = line;

            foreach (Match match in matches)
            {
                string matched = match.Groups[0].Value;
                int matchPosition = right.IndexOf(matched);
                left = right[..matchPosition];
                right = right[(matchPosition + matched.Length)..];

                if (!string.IsNullOrWhiteSpace(left))
                {
                    text.Inlines.Add(new Run { Text = left });
                }

                string hexColor = match.Groups[1].Value;
                string content = match.Groups[2].Value;

                text.Inlines.Add(new Run { Text = content, Foreground = GetSolidColorBrush(hexColor[..7]) });
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

    private static SolidColorBrush GetSolidColorBrush(string hex)
    {
        hex = hex.Replace("#", string.Empty);
        byte r = (byte)Convert.ToUInt32(hex.Substring(0, 2), 16);
        byte g = (byte)Convert.ToUInt32(hex.Substring(2, 2), 16);
        byte b = (byte)Convert.ToUInt32(hex.Substring(4, 2), 16);
        return new SolidColorBrush(Color.FromArgb(255, r, g, b));
    }
}
