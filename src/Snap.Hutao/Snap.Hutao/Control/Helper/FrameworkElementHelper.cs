// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Control.Helper;

[SuppressMessage("", "SH001")]
[DependencyProperty("SquareLength", typeof(double), 0D, nameof(OnSquareLengthChanged), IsAttached = true, AttachedType = typeof(FrameworkElement))]
public sealed partial class FrameworkElementHelper
{
    private static void OnSquareLengthChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
        FrameworkElement element = (FrameworkElement)dp;
        element.Width = (double)e.NewValue;
        element.Height = (double)e.NewValue;
    }
}