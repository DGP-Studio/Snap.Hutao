// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
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
    private readonly LaunchOptions launchOptions;
    private readonly ITaskContext taskContext;

    public async ValueTask<bool> EnsureGameResourceAsync(LaunchScheme launchScheme, IProgress<PackageReplaceStatus> progress)
    {
        if (!launchOptions.TryGetGameDirectoryAndGameFileName(out string? gameFolder, out string? gameFileName))
        {
            return false;
        }

        if (!CheckDirectoryPermissions(gameFolder))
        {
            progress.Report(new(SH.ServiceGameEnsureGameResourceInsufficientDirectoryPermissions));
            return false;
        }

        progress.Report(new(SH.ServiceGameEnsureGameResourceQueryResourceInformation));
        Response<GameResource> response = await serviceProvider
            .GetRequiredService<ResourceClient>()
            .GetResourceAsync(launchScheme)
            .ConfigureAwait(false);

        if (!response.IsOk())
        {
            return false;
        }

        GameResource resource = response.Data;

        if (!launchScheme.ExecutableMatches(gameFileName))
        {
            // We can't start the game when we failed to convert game
            if (!await packageConverter.EnsureGameResourceAsync(launchScheme, resource, gameFolder, progress).ConfigureAwait(false))
            {
                return false;
            }

            // We need to change the gamePath if we switched.
            string exeName = launchScheme.IsOversea ? GenshinImpactFileName : YuanShenFileName;

            await taskContext.SwitchToMainThreadAsync();
            launchOptions.UpdateGamePathAndRefreshEntries(Path.Combine(gameFolder, exeName));
        }

        await packageConverter.EnsureDeprecatedFilesAndSdkAsync(resource, gameFolder).ConfigureAwait(false);
        return true;
    }

    private static bool CheckDirectoryPermissions(string folder)
    {
        try
        {
            string tempFilePath = Path.Combine(folder, $"{Guid.NewGuid():N}.tmp");
            string tempFilePathMove = Path.Combine(folder, $"{Guid.NewGuid():N}.tmp");

            // Test create file
            using (SafeFileHandle handle = File.OpenHandle(tempFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, preallocationSize: 32 * 1024))
            {
                // Test write file
                RandomAccess.Write(handle, "SNAP HUTAO DIRECTORY PERMISSION CHECK"u8, 0);
                RandomAccess.FlushToDisk(handle);
            }

            // Test move file
            File.Move(tempFilePath, tempFilePathMove);

            // Test delete file
            File.Delete(tempFilePathMove);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}