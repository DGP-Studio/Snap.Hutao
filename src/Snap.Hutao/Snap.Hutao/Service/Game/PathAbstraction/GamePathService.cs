// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Locator;
using Snap.Hutao.Service.Game.PathAbstraction;

namespace Snap.Hutao.Service.Game.PathAbstraction;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGamePathService))]
internal sealed partial class GamePathService : IGamePathService
{
    private readonly IServiceProvider serviceProvider;
    private readonly LaunchOptions launchOptions;

    public async ValueTask<ValueResult<bool, string>> SilentGetGamePathAsync()
    {
        // Cannot find in setting
        if (string.IsNullOrEmpty(launchOptions.GamePath))
        {
            IGameLocatorFactory locatorFactory = serviceProvider.GetRequiredService<IGameLocatorFactory>();

            bool isOk;
            string path;

            // Try locate by unity log
            (isOk, path) = await locatorFactory
                .Create(GameLocationSource.UnityLog)
                .LocateGamePathAsync()
                .ConfigureAwait(false);

            if (!isOk)
            {
                // Try locate by registry
                (isOk, path) = await locatorFactory
                    .Create(GameLocationSource.Registry)
                    .LocateGamePathAsync()
                    .ConfigureAwait(false);
            }

            if (isOk)
            {
                // Save result.
                launchOptions.UpdateGamePathAndRefreshEntries(path);
            }
            else
            {
                return new(false, SH.ServiceGamePathLocateFailed);
            }
        }

        if (!string.IsNullOrEmpty(launchOptions.GamePath))
        {
            return new(true, launchOptions.GamePath);
        }
        else
        {
            return new(false, default!);
        }
    }
}