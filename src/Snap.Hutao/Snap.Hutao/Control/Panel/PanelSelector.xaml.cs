// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace Snap.Hutao.Control.Panel;

/// <summary>
/// 面板选择器
/// </summary>
[HighQuality]
internal sealed partial class PanelSelector : SplitButton
{
    private const string List = nameof(List);

    private static readonly DependencyProperty CurrentProperty = Property<PanelSelector>.Depend(nameof(Current), List, OnCurrentChanged);

    private readonly RoutedEventHandler loadedEventHandler;
    private readonly RoutedEventHandler unloadedEventHandler;
    private readonly TypedEventHandler<SplitButton, SplitButtonClickEventArgs> clickEventHandler;
    private readonly RoutedEventHandler menuItemClickEventHandler;

    /// <summary>
    /// 构造一个新的面板选择器
    /// </summary>
    public PanelSelector()
    {
        InitializeComponent();

        loadedEventHandler = OnRootLoaded;
        Loaded += loadedEventHandler;

        clickEventHandler = OnRootClick;
        Click += clickEventHandler;

        menuItemClickEventHandler = OnMenuItemClick;

        unloadedEventHandler = OnRootUnload;
        Unloaded += unloadedEventHandler;
    }

    /// <summary>
    /// 当前选择
    /// </summary>
    public string Current
    {
        get => (string)GetValue(CurrentProperty);
        set => SetValue(CurrentProperty, value);
    }

    private static void InitializeItems(PanelSelector selector)
    {
        MenuFlyout menuFlyout = (MenuFlyout)selector.Flyout;
        int hash = selector.GetHashCode();
        foreach (RadioMenuFlyoutItem item in menuFlyout.Items.Cast<RadioMenuFlyoutItem>())
        {
            item.GroupName = $"{nameof(PanelSelector)}GroupOf@{hash}";
            item.Click += selector.menuItemClickEventHandler;
        }
    }

    private static void OnCurrentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        UpdateRootGlyphAndItemIsCheck((PanelSelector)obj, (string)args.NewValue);
    }

    private static void UpdateRootGlyphAndItemIsCheck(PanelSelector sender, string current)
    {
        RadioMenuFlyoutItem targetItem = (RadioMenuFlyoutItem)((MenuFlyout)sender.Flyout).Items
            .Single(i => (string)i.Tag == current);

        targetItem.IsChecked = true;
        sender.IconPresenter.Glyph = ((FontIcon)targetItem.Icon).Glyph;
    }

    private void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        // because the GroupName shares in global
        // we have to implement a control scoped GroupName.
        PanelSelector selector = (PanelSelector)sender;
        InitializeItems(selector);
        UpdateRootGlyphAndItemIsCheck(selector, Current);
    }

    private void OnRootUnload(object sender, RoutedEventArgs e)
    {
        Loaded -= loadedEventHandler;
        Click -= clickEventHandler;

        foreach (MenuFlyoutItemBase item in ((MenuFlyout)((PanelSelector)sender).Flyout).Items)
        {
            ((RadioMenuFlyoutItem)item).Click -= menuItemClickEventHandler;
        }

        Unloaded -= unloadedEventHandler;
    }

    private void OnRootClick(SplitButton sender, SplitButtonClickEventArgs args)
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

    private void OnMenuItemClick(object sender, RoutedEventArgs e)
    {
        Current = (string)((FrameworkElement)sender).Tag;
    }
}