// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Snap.Hutao.Control;

namespace Snap.Hutao.View.Control;

/// <summary>
/// 底部带有文本的控件
/// </summary>
[HighQuality]
[ContentProperty(Name = nameof(TopContent))]
internal sealed partial class BottomTextControl : ContentControl
{
    private static readonly DependencyProperty TextProperty = Property<BottomTextControl>.Depend(nameof(Text), string.Empty, OnTextChanged);
    private static readonly DependencyProperty TopContentProperty = Property<BottomTextControl>.Depend<UIElement>(nameof(TopContent), default!, OnContentChanged);
    private static readonly DependencyProperty FillProperty = Property<BottomTextControl>.Depend(nameof(Fill), default(Brush), OnFillChanged);

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

    /// <summary>
    /// 填充
    /// </summary>
    public Brush Fill
    {
        get => (Brush)GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    private static void OnTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((BottomTextControl)sender).TextHost.Text = (string)args.NewValue;
    }

    private static void OnContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((BottomTextControl)sender).ContentHost.Content = args.NewValue;
    }

    private static void OnFillChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((BottomTextControl)sender).BackgroundStack.Background = (Brush)args.NewValue;
    }
}
