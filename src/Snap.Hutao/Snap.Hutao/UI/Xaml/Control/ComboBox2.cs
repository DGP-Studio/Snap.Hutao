// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

namespace Snap.Hutao.UI.Xaml.Control;

[DependencyProperty<string>("EnableMemberPath")]
internal sealed partial class ComboBox2 : ComboBox
{
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        if (element is ComboBoxItem comboBoxItem)
        {
            comboBoxItem.SetBinding(IsEnabledProperty, new Binding { Path = new(EnableMemberPath) });
        }
    }
}