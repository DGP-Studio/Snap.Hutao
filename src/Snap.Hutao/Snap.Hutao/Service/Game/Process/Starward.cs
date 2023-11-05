// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.System;

namespace Snap.Hutao.Service.Game.Process;

internal static class Starward
{
    public static async ValueTask LaunchForPlayTimeStatisticsAsync(bool isOversea)
    {
        string gameBiz = isOversea ? "hk4e_global" : "hk4e_cn";
        Uri starwardPlayTimeUri = $"starward://playtime/{gameBiz}".ToUri();
        if (await Launcher.QueryUriSupportAsync(starwardPlayTimeUri, LaunchQuerySupportType.Uri) is LaunchQuerySupportStatus.Available)
        {
            await Launcher.LaunchUriAsync(starwardPlayTimeUri);
        }
    }
}