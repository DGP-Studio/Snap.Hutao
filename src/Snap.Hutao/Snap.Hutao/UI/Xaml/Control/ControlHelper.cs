// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace Snap.Hutao.UI.Xaml.Control;

[SuppressMessage("", "SH001")]
[DependencyProperty<Brush>("Background", PropertyChangedCallbackName = nameof(OnBackgroundChanged), IsAttached = true, TargetType = typeof(Microsoft.UI.Xaml.Controls.Control))]
public sealed partial class ControlHelper
{
    private static void OnBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not Microsoft.UI.Xaml.Controls.Control control)
        {
            return;
        }

        control.Background = (Brush)args.NewValue;
    }
}