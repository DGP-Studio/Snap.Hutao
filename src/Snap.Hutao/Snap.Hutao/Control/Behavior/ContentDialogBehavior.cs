// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI.Behaviors;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

namespace Snap.Hutao.Control.Behavior;

/// <summary>
/// Make ContentDialog's SmokeLayerBackground dsiplay over custom titleBar
/// </summary>
public class ContentDialogBehavior : BehaviorBase<FrameworkElement>
{
    /// <inheritdoc/>
    protected override void OnAssociatedObjectLoaded()
    {
        DependencyObject parent = VisualTreeHelper.GetParent(AssociatedObject);
        DependencyObject child = VisualTreeHelper.GetChild(parent, 2);
        Rectangle smokeLayerBackground = (Rectangle)child;

        smokeLayerBackground.Margin = new Thickness(0);
        smokeLayerBackground.RegisterPropertyChangedCallback(FrameworkElement.MarginProperty, OnMarginChanged);
    }

    private static void OnMarginChanged(DependencyObject sender, DependencyProperty property)
    {
        if (property == FrameworkElement.MarginProperty)
        {
            sender.ClearValue(property);
        }
    }
}