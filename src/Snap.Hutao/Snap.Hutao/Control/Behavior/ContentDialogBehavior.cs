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

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            DependencyObject current = VisualTreeHelper.GetChild(parent, i);
            if (current is Rectangle { Name: "SmokeLayerBackground" } background)
            {
                background.ClearValue(FrameworkElement.MarginProperty);
                background.RegisterPropertyChangedCallback(FrameworkElement.MarginProperty, OnMarginChanged);
                break;
            }
        }
    }

    private static void OnMarginChanged(DependencyObject sender, DependencyProperty property)
    {
        if (property == FrameworkElement.MarginProperty)
        {
            sender.ClearValue(property);
        }
    }
}