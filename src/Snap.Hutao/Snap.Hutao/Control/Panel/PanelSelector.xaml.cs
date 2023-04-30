// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Control.Panel;

/// <summary>
/// 面板选择器
/// </summary>
[HighQuality]
internal sealed partial class PanelSelector : SplitButton
{
    private const string List = nameof(List);

    private static readonly DependencyProperty CurrentProperty = Property<PanelSelector>.Depend(nameof(Current), List, OnCurrentChanged);

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

    private static void OnCurrentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        OnCurrentChanged((PanelSelector)obj, (string)args.NewValue);
    }

    private static void OnCurrentChanged(PanelSelector sender, string current)
    {
        MenuFlyout menuFlyout = (MenuFlyout)sender.RootSplitButton.Flyout;
        RadioMenuFlyoutItem targetItem = menuFlyout.Items
            .Cast<RadioMenuFlyoutItem>()
            .Single(i => (string)i.Tag == current);
        targetItem.IsChecked = true;
        sender.IconPresenter.Glyph = ((FontIcon)targetItem.Icon).Glyph;
    }

    private void OnRootControlLoaded(object sender, RoutedEventArgs e)
    {
        // because the GroupName shares in global
        // we have to implement a control scoped GroupName.
        PanelSelector selector = (PanelSelector)sender;
        MenuFlyout menuFlyout = (MenuFlyout)selector.RootSplitButton.Flyout;
        int hash = GetHashCode();
        foreach (RadioMenuFlyoutItem item in menuFlyout.Items.Cast<RadioMenuFlyoutItem>())
        {
            item.GroupName = $"{nameof(PanelSelector)}GroupOf@{hash}";
        }

        OnCurrentChanged(selector, Current);
    }

    private void SplitButtonClick(SplitButton sender, SplitButtonClickEventArgs args)
    {
        MenuFlyout menuFlyout = (MenuFlyout)sender.Flyout;

        int i = 0;
        for (; i < menuFlyout.Items.Count; i++)
        {
            if ((string)menuFlyout.Items[i].Tag == Current)
            {
                break;
            }
        }

        ++i;
        i %= menuFlyout.Items.Count; // move the count index to 0

        RadioMenuFlyoutItem item = (RadioMenuFlyoutItem)menuFlyout.Items[i];
        item.IsChecked = true;
        Current = (string)item.Tag;
    }

    private void RadioMenuFlyoutItemClick(object sender, RoutedEventArgs e)
    {
        RadioMenuFlyoutItem item = (RadioMenuFlyoutItem)sender;
        Current = (string)item.Tag;
    }
}