// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;

namespace Snap.Hutao.UI.Xaml;

[SuppressMessage("", "SH001")]
[DependencyProperty<double>("SquareLength", DefaultValue = 0D, PropertyChangedCallbackName = nameof(OnSquareLengthChanged), IsAttached = true, TargetType = typeof(FrameworkElement), NotNull = true)]
public sealed partial class FrameworkElementHelper
{
    private static void OnSquareLengthChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
        FrameworkElement element = (FrameworkElement)dp;
        element.Width = (double)e.NewValue;
        element.Height = (double)e.NewValue;
    }
}