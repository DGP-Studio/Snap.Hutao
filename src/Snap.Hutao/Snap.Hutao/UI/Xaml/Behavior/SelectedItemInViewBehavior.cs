// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Behavior;

internal sealed class SelectedItemInViewBehavior : BehaviorBase<ListViewBase>
{
    protected override bool Initialize()
    {
        if (AssociatedObject.SelectedItem is { } item)
        {
            AssociatedObject.SmoothScrollIntoViewWithItemAsync(item, ScrollItemPlacement.Center).SafeForget();
        }

        return true;
    }

    protected override void OnAssociatedObjectLoaded()
    {
        if (AssociatedObject.SelectedItem is { } item)
        {
            AssociatedObject.SmoothScrollIntoViewWithItemAsync(item, ScrollItemPlacement.Center).SafeForget();
        }
    }
}