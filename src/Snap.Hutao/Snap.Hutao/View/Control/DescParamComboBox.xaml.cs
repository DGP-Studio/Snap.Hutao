// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core;
using Snap.Hutao.Model.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace Snap.Hutao.View.Control;

/// <summary>
/// 描述参数组合框
/// </summary>
public sealed partial class DescParamComboBox : UserControl
{
    private static readonly DependencyProperty SourceProperty = Property<DescParamComboBox>
        .Depend<IList<LevelParam<string, ParameterInfo>>>(nameof(Source), default!, OnSourceChanged);

    /// <summary>
    /// 构造一个新的描述参数组合框
    /// </summary>
    public DescParamComboBox()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 技能列表
    /// </summary>
    public IList<LevelParam<string, ParameterInfo>> Source
    {
        get { return (IList<LevelParam<string, ParameterInfo>>)GetValue(SourceProperty); }
        set { SetValue(SourceProperty, value); }
    }

    private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        // Some of the {x:Bind} feature is not working properly,
        // so we use this simple code behind approach to achieve selection function
        if (sender is DescParamComboBox descParamComboBox)
        {
            if (args.NewValue != args.OldValue && args.NewValue is IList<LevelParam<string, ParameterInfo>> list)
            {
                descParamComboBox.ItemHost.ItemsSource = list;
                descParamComboBox.ItemHost.SelectedIndex = 0;

                descParamComboBox.DetailsHost.ItemsSource = list.FirstOrDefault()?.Parameters;
            }
        }
    }

    private void ItemHostSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedIndex >= 0)
        {
            DetailsHost.ItemsSource = Source[comboBox.SelectedIndex]?.Parameters;
        }
    }
}
