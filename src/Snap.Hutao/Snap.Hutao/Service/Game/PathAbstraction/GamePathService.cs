﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Locator;

namespace Snap.Hutao.Service.Game.PathAbstraction;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGamePathService))]
internal sealed partial class GamePathService : IGamePathService
{
    private readonly IGameLocatorFactory gameLocatorFactory;
    private readonly LaunchOptions launchOptions;

    public async ValueTask<ValueResult<bool, string>> SilentGetGamePathAsync()
    {
        // Cannot find in setting
        if (string.IsNullOrEmpty(launchOptions.GamePath))
        {
            // Try locate by unity log
            (bool isOk, string path) = await gameLocatorFactory.LocateAsync(GameLocationSource.UnityLog).ConfigureAwait(false);

            if (!isOk)
            {
                // Try locate by registry
                (isOk, path) = await gameLocatorFactory.LocateAsync(GameLocationSource.Registry).ConfigureAwait(false);
            }

            if (isOk)
            {
                // Save result.
                launchOptions.UpdateGamePath(path);
            }
            else
            {
                return new(false, SH.ServiceGamePathLocateFailed);
            }
        }

        if (string.IsNullOrEmpty(launchOptions.GamePath))
        {
            return new(false, default!);
        }

        return new(true, launchOptions.GamePath);
    }
}