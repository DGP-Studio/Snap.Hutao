// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Setting;
using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using WinRT;

namespace Snap.Hutao.UI.Xaml.Control;

[DependencyProperty<string>("Current", DefaultValue = List)]
[DependencyProperty<string>("LocalSettingKeySuffixForCurrent")]
[DependencyProperty<string>("LocalSettingKeyExtraForCurrent", DefaultValue = "")]
internal sealed partial class LayoutSwitch : Segmented
{
    public const string List = nameof(List);
    public const string Grid = nameof(Grid);

    private static readonly FrozenDictionary<int, string> IndexTypeMap = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(0, List),
        KeyValuePair.Create(1, Grid),
    ]);

    private readonly RoutedEventHandler loadedEventHandler = OnRootLoaded;
    private readonly RoutedEventHandler unloadedEventHandler = OnRootUnload;
    private readonly long selectedIndexChangedCallbackToken;

    public LayoutSwitch()
    {
        InitializeComponent();

        Loaded += loadedEventHandler;
        Unloaded += unloadedEventHandler;

        selectedIndexChangedCallbackToken = RegisterPropertyChangedCallback(SelectedIndexProperty, OnSelectedIndexChanged);
    }

    private static void OnSelectedIndexChanged(DependencyObject sender, DependencyProperty dp)
    {
        LayoutSwitch selector = sender.As<LayoutSwitch>();
        selector.Current = IndexTypeMap[Unsafe.Unbox<int>(selector.GetValue(dp))];

        if (!string.IsNullOrEmpty(selector.LocalSettingKeySuffixForCurrent))
        {
            LocalSetting.Set(GetSettingKey(selector), selector.Current);
        }
    }

    private static void OnRootLoaded(object sender, RoutedEventArgs e)
    {
        LayoutSwitch selector = sender.As<LayoutSwitch>();

        if (string.IsNullOrEmpty(selector.LocalSettingKeySuffixForCurrent))
        {
            return;
        }

        string? value = LocalSetting.Get(GetSettingKey(selector), selector.Current);
        selector.Current = value;

        selector.SelectedItem = selector.Items.Cast<SegmentedItem>().Single(item => string.Equals(item.Tag.As<string>(), selector.Current, StringComparison.Ordinal));
    }

    private static void OnRootUnload(object sender, RoutedEventArgs e)
    {
        LayoutSwitch selector = sender.As<LayoutSwitch>();
        selector.UnregisterPropertyChangedCallback(SelectedIndexProperty, selector.selectedIndexChangedCallbackToken);
        selector.Unloaded -= selector.unloadedEventHandler;
    }

    private static string GetSettingKey(LayoutSwitch selector)
    {
        return $"Control.PanelSelector.{selector.LocalSettingKeySuffixForCurrent}{selector.LocalSettingKeyExtraForCurrent}";
    }
}