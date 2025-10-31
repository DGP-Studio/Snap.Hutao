// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.FileSystem;
using System.IO;

namespace Snap.Hutao.Service.Game.Configuration;

[Service(ServiceLifetime.Singleton, typeof(IGameChannelOptionsService))]
internal sealed partial class GameChannelOptionsService : IGameChannelOptionsService
{
    private readonly IGameConfigurationFileService gameConfigurationFileService;
    private readonly LaunchOptions launchOptions;

    [GeneratedConstructor]
    public partial GameChannelOptionsService(IServiceProvider serviceProvider);

    public ChannelOptions GetChannelOptions()
    {
        const string LockTrace = $"{nameof(GameChannelOptionsService)}.{nameof(GetChannelOptions)}";
        GameFileSystemErrorKind errorKind = launchOptions.TryGetGameFileSystem(LockTrace, out IGameFileSystem? gameFileSystem);
        switch (errorKind)
        {
            case GameFileSystemErrorKind.GamePathNullOrEmpty:
                return ChannelOptions.GamePathNullOrEmpty();
            case GameFileSystemErrorKind.GamePathLocked:
                return ChannelOptions.GamePathLocked(launchOptions.GamePathEntry.Value?.Path ?? string.Empty);
        }

        ArgumentNullException.ThrowIfNull(gameFileSystem);
        using (gameFileSystem)
        {
            string configFilePath = gameFileSystem.GetGameConfigurationFilePath();

            if (!File.Exists(configFilePath))
            {
                // Try restore the configuration file if it does not exist
                // The configuration file may be deleted by an incompatible launcher(e.g., CN client but OS launcher and vice versa)
                gameConfigurationFileService.Restore(configFilePath, gameFileSystem.IsExecutableOversea());

                if (!File.Exists(configFilePath))
                {
                    return ChannelOptions.ConfigurationFileNotFound(configFilePath);
                }
            }

            string scriptVersionFilePath = gameFileSystem.GetScriptVersionFilePath();

            if (!File.Exists(scriptVersionFilePath))
            {
                // Try to fix ScriptVersion by reading game_version from the configuration file
                // If the configuration file and ScriptVersion file are both missing, the game content is corrupted
                if (!GameScriptVersion.Patch(configFilePath, scriptVersionFilePath))
                {
                    return ChannelOptions.GameContentCorrupted(gameFileSystem.GetGameDirectory());
                }
            }

            return GameConfiguration.Read(gameFileSystem);
        }
    }
}