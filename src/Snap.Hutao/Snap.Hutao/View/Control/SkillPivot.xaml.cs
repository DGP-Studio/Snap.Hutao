// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control.Segmented;
using System.Collections;

namespace Snap.Hutao.View.Control;

/// <summary>
/// 技能展柜
/// </summary>
[HighQuality]
[DependencyProperty("Skills", typeof(IList), null, nameof(OnSkillsChanged))]
[DependencyProperty("Selected", typeof(object))]
[DependencyProperty("ItemTemplate", typeof(DataTemplate))]
internal sealed partial class SkillPivot : UserControl
{
    /// <summary>
    /// 创建一个新的技能展柜
    /// </summary>
    public SkillPivot()
    {
        InitializeComponent();
    }

    private static void OnSkillsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is SkillPivot skillPivot)
        {
            if (args.OldValue != args.NewValue && args.NewValue as IList is [object target, ..] list)
            {
                skillPivot.Selected = target;
                skillPivot.SkillSelectorSegmented.ItemsSource = list;
                skillPivot.SkillSelectorSegmented.SelectedItem = target;
            }
        }
    }

    private void OnSkillSelectorSegmentedSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Selected = ((Segmented)sender).SelectedItem;
    }
}