// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace Snap.Hutao.Control.Collection.Selector;

[DependencyProperty("EnableMemberPath", typeof(string))]
internal sealed partial class ComboBox2 : ComboBox
{
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        if (element is ComboBoxItem comboBoxItem)
        {
            Binding binding = new() { Path = new(EnableMemberPath) };
            comboBoxItem.SetBinding(IsEnabledProperty, binding);
        }

        base.PrepareContainerForItemOverride(element, item);
    }
}