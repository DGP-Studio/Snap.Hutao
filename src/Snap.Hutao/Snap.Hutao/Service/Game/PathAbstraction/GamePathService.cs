// Copyright (c) DGP Studio. All rights reserved.
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
            // Try to locate by unity log
            if (await gameLocatorFactory.LocateAsync(GameLocationSource.UnityLog).ConfigureAwait(false) is (true, { } path1))
            {
                launchOptions.UpdateGamePath(path1);
                return new(true, launchOptions.GamePath);
            }

            // Try to locate by registry
            if (await gameLocatorFactory.LocateAsync(GameLocationSource.Registry).ConfigureAwait(false) is (true, { } path2))
            {
                launchOptions.UpdateGamePath(path2);
                return new(true, launchOptions.GamePath);
            }

            // If it's still null or empty
            if (string.IsNullOrEmpty(launchOptions.GamePath))
            {
                return new(false, SH.ServiceGamePathLocateFailed);
            }
        }

        return new(true, launchOptions.GamePath);
    }
}