// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao;

// Place to store critical application state
internal static class XamlApplicationLifetime
{
    public static bool DispatcherQueueInitialized { get; set; }

    public static bool CultureInfoInitialized { get; set; }

    public static bool LaunchedWithNotifyIcon { get; set; }

    public static bool IsFirstRunAfterUpdate { get; set; }

    public static bool Exiting { get; set; }
}