// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core.Setting;

namespace Snap.Hutao.Control.Panel;

/// <summary>
/// 面板选择器
/// </summary>
[HighQuality]
[DependencyProperty("Current", typeof(string), List)]
[DependencyProperty("LocalSettingKeySuffixForCurrent", typeof(string))]
[DependencyProperty("LocalSettingKeyExtraForCurrent", typeof(string), "")]
internal sealed partial class PanelSelector : Segmented
{
    public const string List = nameof(List);
    public const string Grid = nameof(Grid);

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

    private static void OnSelectedIndexChanged(DependencyObject sender, DependencyProperty dp)
    {
        PanelSelector selector = (PanelSelector)sender;
        selector.Current = IndexTypeMap[(int)selector.GetValue(dp)];

        if (!string.IsNullOrEmpty(selector.LocalSettingKeySuffixForCurrent))
        {
            LocalSetting.Set(GetSettingKey(selector), selector.Current);
        }
    }

    private static void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        PanelSelector selector = (PanelSelector)sender;

        if (string.IsNullOrEmpty(selector.LocalSettingKeySuffixForCurrent))
        {
            return;
        }

        string value = LocalSetting.Get(GetSettingKey(selector), selector.Current);
        selector.Current = value;

        selector.SelectedItem = selector.Items.Cast<SegmentedItem>().Single(item => (string)item.Tag == selector.Current);
    }

    private static void OnRootUnload(object sender, RoutedEventArgs e)
    {
        PanelSelector selector = (PanelSelector)sender;
        selector.UnregisterPropertyChangedCallback(SelectedIndexProperty, selector.selectedIndexChangedCallbackToken);
        selector.Unloaded -= selector.unloadedEventHandler;
    }

    private static string GetSettingKey(PanelSelector selector)
    {
        return $"Control.PanelSelector.{selector.LocalSettingKeySuffixForCurrent}{selector.LocalSettingKeyExtraForCurrent}";
    }
}