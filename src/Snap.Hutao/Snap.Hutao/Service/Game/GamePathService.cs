// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Locator;

namespace Snap.Hutao.Service.Game;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGamePathService))]
internal sealed partial class GamePathService : IGamePathService
{
    private readonly IServiceProvider serviceProvider;
    private readonly AppOptions appOptions;

    public async ValueTask<ValueResult<bool, string>> SilentGetGamePathAsync()
    {
        // Cannot find in setting
        if (string.IsNullOrEmpty(appOptions.GamePath))
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
                appOptions.GamePath = path;
            }
            else
            {
                return new(false, SH.ServiceGamePathLocateFailed);
            }
        }

        if (!string.IsNullOrEmpty(appOptions.GamePath))
        {
            return new(true, appOptions.GamePath);
        }
        else
        {
            return new(false, default!);
        }
    }
}