// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Control.Helper;

[SuppressMessage("", "SH001")]
[DependencyProperty("VisibilityObject", typeof(object), null, nameof(OnVisibilityObjectChanged), IsAttached = true, AttachedType = typeof(UIElement))]
[DependencyProperty("OpacityObject", typeof(object), null, nameof(OnOpacityObjectChanged), IsAttached = true, AttachedType = typeof(UIElement))]
public sealed partial class UIElementHelper
{
    private static void OnVisibilityObjectChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
        UIElement element = (UIElement)dp;
        element.Visibility = e.NewValue is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private static void OnOpacityObjectChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
        UIElement element = (UIElement)dp;
        element.Opacity = e.NewValue is null ? 0D : 1D;
    }
}