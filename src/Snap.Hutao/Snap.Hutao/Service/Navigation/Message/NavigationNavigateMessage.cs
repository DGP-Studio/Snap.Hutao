// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Navigation.Message;

internal sealed class NavigationNavigateMessage
{
    public required Type PageType { get; set; }

    public required INavigationCompletionSource Data { get; set; }

    public required bool SyncNavigationViewItem { get; set; }

    public NavigationResult Result { get; set; }
}