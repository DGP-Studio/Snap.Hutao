// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.Control.Helper;

[SuppressMessage("", "SH001")]
[DependencyProperty("IsItemsEnabled", typeof(bool), true, nameof(OnIsItemsEnabledChanged), IsAttached = true, AttachedType = typeof(SettingsExpander))]
public sealed partial class SettingsExpanderHelper
{
    private static void OnIsItemsEnabledChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
        foreach (object item in ((SettingsExpander)dp).Items)
        {
            if (item is Microsoft.UI.Xaml.Controls.Control control)
            {
                control.IsEnabled = (bool)e.NewValue;
            }
        }
    }
}
