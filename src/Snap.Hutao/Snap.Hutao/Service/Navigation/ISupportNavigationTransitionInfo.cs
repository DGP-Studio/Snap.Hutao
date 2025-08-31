// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media.Animation;

namespace Snap.Hutao.Service.Navigation;

internal interface ISupportNavigationTransitionInfo
{
    NavigationTransitionInfo? TransitionInfo { get; }
}