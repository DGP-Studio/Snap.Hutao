// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;
using Snap.Hutao.Web.Response;
using System.IO;
using static Snap.Hutao.Service.Game.GameConstants;

namespace Snap.Hutao.Service.Game.Package;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGamePackageService))]
internal sealed partial class GamePackageService : IGamePackageService
{
    private readonly PackageConverter packageConverter;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    public async ValueTask<bool> EnsureGameResourceAsync(LaunchScheme launchScheme, IProgress<PackageReplaceStatus> progress)
    {
        if (!appOptions.TryGetGameFolderAndFileName(out string? gameFolder, out string? gameFileName))
        {
            return false;
        }

        progress.Report(new(SH.ServiceGameEnsureGameResourceQueryResourceInformation));
        Response<GameResource> response = await serviceProvider
            .GetRequiredService<ResourceClient>()
            .GetResourceAsync(launchScheme)
            .ConfigureAwait(false);

        if (response.IsOk())
        {
            GameResource resource = response.Data;

            if (!launchScheme.ExecutableMatches(gameFileName))
            {
                bool replaced = await packageConverter
                    .EnsureGameResourceAsync(launchScheme, resource, gameFolder, progress)
                    .ConfigureAwait(false);

                if (replaced)
                {
                    // We need to change the gamePath if we switched.
                    string exeName = launchScheme.IsOversea ? GenshinImpactFileName : YuanShenFileName;

                    await taskContext.SwitchToMainThreadAsync();
                    appOptions.GamePath = Path.Combine(gameFolder, exeName);
                }
                else
                {
                    // We can't start the game
                    // when we failed to convert game
                    return false;
                }
            }

            await packageConverter.EnsureDeprecatedFilesAndSdkAsync(resource, gameFolder).ConfigureAwait(false);

            return true;
        }

        return false;
    }
}