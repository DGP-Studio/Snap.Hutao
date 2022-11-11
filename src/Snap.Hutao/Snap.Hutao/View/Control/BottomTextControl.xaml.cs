// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Snap.Hutao.Core;

namespace Snap.Hutao.View.Control;

/// <summary>
/// 底部带有文本的控件
/// </summary>
[ContentProperty(Name = nameof(TopContent))]
public sealed partial class BottomTextControl : ContentControl
{
    private static readonly DependencyProperty TextProperty = Property<BottomTextControl>.Depend(nameof(Text), string.Empty, OnTextChanged);
    private static readonly DependencyProperty TopContentProperty = Property<BottomTextControl>.Depend<UIElement>(nameof(TopContent), default!, OnContentChanged2);

    /// <summary>
    /// 构造一个新的底部带有文本的控件
    /// </summary>
    public BottomTextControl()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 顶部内容
    /// </summary>
    public UIElement TopContent
    {
        get => (UIElement)GetValue(TopContentProperty);
        set => SetValue(TopContentProperty, value);
    }

    /// <summary>
    /// 文本
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs dp)
    {
        ((BottomTextControl)sender).TextHost.Text = (string)dp.NewValue;
    }

    private static void OnContentChanged2(DependencyObject sender, DependencyPropertyChangedEventArgs dp)
    {
        ((BottomTextControl)sender).ContentHost.Content = dp.NewValue;
    }
}
