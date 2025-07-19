// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Behavior;

internal sealed class ToggleSwitchHeaderHitTestVisibleBehavior : BehaviorBase<ToggleSwitch>
{
    protected override void OnAssociatedObjectLoaded()
    {
        AssociatedObject.FindDescendant("HeaderContentPresenter")!.IsHitTestVisible = true;
    }
}