// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao;

internal static class XamlApplicationLifetime
{
    public static bool LaunchedWithNotifyIcon { get; set; }

    public static bool Exiting { get; set; }

    public static bool IsFirstRunAfterUpdate { get; set; }
}