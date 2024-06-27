// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Model.Metadata;

namespace Snap.Hutao.UI.Xaml.View.Specialized;

[HighQuality]
[DependencyProperty("Source", typeof(List<LevelParameters<string, ParameterDescription>>), default!, nameof(OnSourceChanged))]
[DependencyProperty("SelectedItem", typeof(LevelParameters<string, ParameterDescription>), default!)]
[DependencyProperty("PreferredSelectedIndex", typeof(int), 0)]
internal sealed partial class DescParamComboBox : UserControl
{
    public DescParamComboBox()
    {
        InitializeComponent();
    }

    private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is DescParamComboBox descParamComboBox)
        {
            if (args.NewValue != args.OldValue && args.NewValue is List<LevelParameters<string, ParameterDescription>> list)
            {
                descParamComboBox.Bindings.Update();
                descParamComboBox.SelectedItem = list.ElementAtOrDefault(descParamComboBox.PreferredSelectedIndex) ?? list.LastOrDefault();
            }
        }
    }
}