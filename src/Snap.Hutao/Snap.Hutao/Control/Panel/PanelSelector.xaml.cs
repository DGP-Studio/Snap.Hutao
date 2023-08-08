// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Labs.WinUI;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.Control.Panel;

/// <summary>
/// 面板选择器
/// </summary>
[HighQuality]
[DependencyProperty("Current", typeof(string), List)]
internal sealed partial class PanelSelector : Segmented
{
    private const string List = nameof(List);
    private const string Grid = nameof(Grid);

    private static readonly Dictionary<int, string> IndexTypeMap = new()
    {
        [0] = List,
        [1] = Grid,
    };

    private readonly RoutedEventHandler loadedEventHandler;
    private readonly RoutedEventHandler unloadedEventHandler;
    private readonly long selectedIndexChangedCallbackToken;

    /// <summary>
    /// 构造一个新的面板选择器
    /// </summary>
    public PanelSelector()
    {
        InitializeComponent();

        loadedEventHandler = OnRootLoaded;
        Loaded += loadedEventHandler;

        unloadedEventHandler = OnRootUnload;
        Unloaded += unloadedEventHandler;

        selectedIndexChangedCallbackToken = RegisterPropertyChangedCallback(SelectedIndexProperty, OnSelectedIndexChanged);
    }

    private void OnSelectedIndexChanged(DependencyObject sender, DependencyProperty dp)
    {
        Current = IndexTypeMap[(int)GetValue(dp)];
    }

    private void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        PanelSelector selector = (PanelSelector)sender;
        selector.SelectedItem = selector.Items.Cast<SegmentedItem>().Single(item => (string)item.Tag == Current);
    }

    private void OnRootUnload(object sender, RoutedEventArgs e)
    {
        UnregisterPropertyChangedCallback(SelectedIndexProperty, selectedIndexChangedCallbackToken);
        Loaded -= loadedEventHandler;
        Unloaded -= unloadedEventHandler;
    }
}