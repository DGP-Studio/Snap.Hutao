// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml.Control;

[SuppressMessage("", "SH001")]
[DependencyProperty("IsDisabled1", typeof(bool), default(bool), nameof(OnIsDisabledChanged), IsAttached = true, AttachedType = typeof(Microsoft.UI.Xaml.Controls.Control))]
[DependencyProperty("IsDisabled2", typeof(bool), default(bool), nameof(OnIsDisabledChanged), IsAttached = true, AttachedType = typeof(Microsoft.UI.Xaml.Controls.Control))]
[DependencyProperty("IsDisabled3", typeof(bool), default(bool), nameof(OnIsDisabledChanged), IsAttached = true, AttachedType = typeof(Microsoft.UI.Xaml.Controls.Control))]
public sealed partial class ControlHelper
{
    private static void OnIsDisabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not Microsoft.UI.Xaml.Controls.Control control)
        {
            return;
        }

        control.IsEnabled = !GetAnyDisabled(control);
    }

    private static bool GetAnyDisabled(Microsoft.UI.Xaml.Controls.Control control)
    {
        return GetIsDisabled1(control) || GetIsDisabled2(control) || GetIsDisabled3(control);
    }
}