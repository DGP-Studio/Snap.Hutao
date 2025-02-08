// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace Snap.Hutao.UI.Xaml.Behavior;

internal sealed class PreventPerformActionsBehavior : BehaviorBase<UIElement>
{
    protected override bool Initialize()
    {
        AssociatedObject.PointerPressed += OnPointerEvent;
        AssociatedObject.PointerReleased += OnPointerEvent;
        AssociatedObject.RightTapped += OnRightTapped;
        return true;
    }

    protected override bool Uninitialize()
    {
        AssociatedObject.PointerPressed -= OnPointerEvent;
        AssociatedObject.PointerReleased -= OnPointerEvent;
        AssociatedObject.RightTapped -= OnRightTapped;
        return true;
    }

    private static void OnPointerEvent(object sender, PointerRoutedEventArgs e)
    {
        e.Handled = true;
    }

    private static void OnRightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        e.Handled = true;
    }
}