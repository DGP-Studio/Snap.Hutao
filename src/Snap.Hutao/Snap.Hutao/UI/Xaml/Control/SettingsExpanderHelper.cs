// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Control;

[SuppressMessage("", "SH001")]
[DependencyProperty<bool>("IsItemsEnabled", DefaultValue = true, PropertyChangedCallbackName = nameof(OnIsItemsEnabledChanged), IsAttached = true, TargetType = typeof(SettingsExpander), NotNull = true)]
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