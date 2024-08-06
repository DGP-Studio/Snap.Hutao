// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint;

internal interface IInfrastructureWallpaperEndpoints : IInfrastructureRootAccess
{
    public string WallpaperBing()
    {
        return $"{Root}/wallpaper/bing";
    }

    public string WallpaperGenshinLauncher()
    {
        return $"{Root}/wallpaper/genshinlauncher";
    }

    public string WallpaperToday()
    {
        return $"{Root}/wallpaper/today";
    }
}