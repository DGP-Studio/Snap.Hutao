// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Control.Helper;

[SuppressMessage("", "SH001")]
[DependencyProperty("PaneCornerRadius", typeof(CornerRadius), default, nameof(OnPaneCornerRadiusChanged), IsAttached = true, AttachedType = typeof(NavigationView))]
public sealed partial class NavigationViewHelper
{
    private static void OnPaneCornerRadiusChanged(DependencyObject dp, DependencyPropertyChangedEventArgs args)
    {
        NavigationView navigationView = (NavigationView)dp;
        CornerRadius newValue = (CornerRadius)args.NewValue;

        if (navigationView.IsLoaded)
        {
            SetNavigationViewPaneCornerRadius(navigationView, newValue);
            return;
        }

        navigationView.Loaded += (s, e) =>
        {
            NavigationView loadedNavigationView = (NavigationView)s;
            SetNavigationViewPaneCornerRadius(loadedNavigationView, newValue);
        };
    }

    private static void SetNavigationViewPaneCornerRadius(NavigationView navigationView, CornerRadius value)
    {
        if (navigationView.FindDescendant("RootSplitView") is SplitView splitView)
        {
            splitView.CornerRadius = value;
        }
    }
}
