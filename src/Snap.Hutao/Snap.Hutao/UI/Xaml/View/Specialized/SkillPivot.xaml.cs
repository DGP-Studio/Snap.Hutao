// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections;

namespace Snap.Hutao.UI.Xaml.View.Specialized;

[DependencyProperty("Skills", typeof(IList), null, nameof(OnSkillsChanged))]
[DependencyProperty("Selected", typeof(object))]
[DependencyProperty("ItemTemplate", typeof(DataTemplate))]
internal sealed partial class SkillPivot : UserControl
{
    public SkillPivot()
    {
        InitializeComponent();
    }

    private static void OnSkillsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is SkillPivot skillPivot)
        {
            if (args.OldValue != args.NewValue && args.NewValue as IList is [{ } target, ..])
            {
                skillPivot.Bindings.Update();
                skillPivot.Selected = target;
            }
        }
    }
}