// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Model.Metadata;

namespace Snap.Hutao.View.Control;

/// <summary>
/// 描述参数组合框
/// </summary>
[HighQuality]
[DependencyProperty("Source", typeof(List<LevelParameters<string, ParameterDescription>>), default!, nameof(OnSourceChanged))]
[DependencyProperty("PreferredSelectedIndex", typeof(int), 0)]
internal sealed partial class DescParamComboBox : UserControl
{
    /// <summary>
    /// 构造一个新的描述参数组合框
    /// </summary>
    public DescParamComboBox()
    {
        InitializeComponent();
    }

    private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        // Some of the {x:Bind} feature is not working properly,
        // so we use this simple code behind approach to achieve selection function
        if (sender is DescParamComboBox descParamComboBox)
        {
            if (args.NewValue != args.OldValue && args.NewValue is List<LevelParameters<string, ParameterDescription>> list)
            {
                descParamComboBox.ItemHost.ItemsSource = list;
                descParamComboBox.ItemHost.SelectedIndex = Math.Min(descParamComboBox.PreferredSelectedIndex, list.Count - 1);
            }
        }
    }

    private void ItemHostSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox { SelectedIndex: >= 0 } comboBox)
        {
            DetailsHost.ItemsSource = Source[comboBox.SelectedIndex]?.Parameters;
        }
    }
}
