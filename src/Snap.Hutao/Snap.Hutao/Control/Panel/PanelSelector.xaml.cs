// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Control.Panel;

/// <summary>
/// 面板选择器
/// </summary>
public sealed partial class PanelSelector : UserControl
{
    private static readonly DependencyProperty CurrentProperty = Property<PanelSelector>.Depend(nameof(Current), "List");

    /// <summary>
    /// 构造一个新的面板选择器
    /// </summary>
    public PanelSelector()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 当前选择
    /// </summary>
    public string Current
    {
        get => (string)GetValue(CurrentProperty);
        set => SetValue(CurrentProperty, value);
    }

    private void SplitButtonLoaded(object sender, RoutedEventArgs e)
    {
        MenuFlyout menuFlyout = (MenuFlyout)((SplitButton)sender).Flyout;
        ((RadioMenuFlyoutItem)menuFlyout.Items[0]).IsChecked = true;
    }

    private void SplitButtonClick(SplitButton sender, SplitButtonClickEventArgs args)
    {
        MenuFlyout menuFlyout = (MenuFlyout)sender.Flyout;
        int i = 0;
        for (; i < menuFlyout.Items.Count; i++)
        {
            RadioMenuFlyoutItem current = (RadioMenuFlyoutItem)menuFlyout.Items[i];
            if (current.IsChecked)
            {
                break;
            }
        }

        i++;

        if (i > menuFlyout.Items.Count)
        {
            i = 1;
        }

        if (i == menuFlyout.Items.Count)
        {
            i = 0;
        }

        RadioMenuFlyoutItem item = (RadioMenuFlyoutItem)menuFlyout.Items[i];
        item.IsChecked = true;
        UpdateState(item);
    }

    private void RadioMenuFlyoutItemClick(object sender, RoutedEventArgs e)
    {
        RadioMenuFlyoutItem item = (RadioMenuFlyoutItem)sender;
        UpdateState(item);
    }

    private void UpdateState(RadioMenuFlyoutItem item)
    {
        Current = (string)item.Tag;
        IconPresenter.Glyph = ((FontIcon)item.Icon).Glyph;
    }
}