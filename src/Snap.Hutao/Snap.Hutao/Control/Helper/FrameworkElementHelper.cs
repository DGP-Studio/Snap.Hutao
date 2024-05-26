// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.Control.Helper;

[SuppressMessage("", "SH001")]
[DependencyProperty("SquareLength", typeof(double), 0D, nameof(OnSquareLengthChanged), IsAttached = true, AttachedType = typeof(FrameworkElement))]
[DependencyProperty("IsActualThemeBindingEnabled", typeof(bool), false, nameof(OnIsActualThemeBindingEnabled), IsAttached = true, AttachedType = typeof(FrameworkElement))]
[DependencyProperty("ActualTheme", typeof(ElementTheme), ElementTheme.Default, IsAttached = true, AttachedType = typeof(FrameworkElement))]
public sealed partial class FrameworkElementHelper
{
    private static void OnSquareLengthChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
        FrameworkElement element = (FrameworkElement)dp;
        element.Width = (double)e.NewValue;
        element.Height = (double)e.NewValue;
    }

    private static void OnIsActualThemeBindingEnabled(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
        FrameworkElement element = (FrameworkElement)dp;
        if ((bool)e.NewValue)
        {
            element.ActualThemeChanged += OnActualThemeChanged;
        }
        else
        {
            element.ActualThemeChanged -= OnActualThemeChanged;
        }

        static void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetActualTheme(sender, sender.ActualTheme);
        }
    }
}