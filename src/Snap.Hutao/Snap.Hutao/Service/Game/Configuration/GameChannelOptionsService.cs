// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.IO;

namespace Snap.Hutao.Service.Game.Configuration;

[ConstructorGenerated]
[Service(ServiceLifetime.Singleton, typeof(IGameChannelOptionsService))]
internal sealed partial class GameChannelOptionsService : IGameChannelOptionsService
{
    private readonly IGameConfigurationFileService gameConfigurationFileService;
    private readonly LaunchOptions launchOptions;

    public ChannelOptions GetChannelOptions()
    {
        if (!launchOptions.TryGetGameFileSystem(out IGameFileSystem? gameFileSystem))
        {
            return ChannelOptions.GamePathNullOrEmpty();
        }

        using (gameFileSystem)
        {
            string configFilePath = gameFileSystem.GetGameConfigurationFilePath();
            if (!File.Exists(configFilePath))
            {
                // Try restore the configuration file if it does not exist
                // The configuration file may be deleted by an incompatible launcher(e.g., CN client but OS launcher and vice versa)
                gameConfigurationFileService.Restore(configFilePath, gameFileSystem.IsExecutableOversea());
            }

            if (!File.Exists(gameFileSystem.GetScriptVersionFilePath()))
            {
                // Try to fix ScriptVersion by reading game_version from the configuration file
                // If the configuration file and ScriptVersion file are both missing, the game content is corrupted
                if (!gameFileSystem.TryFixScriptVersion())
                {
                    return ChannelOptions.GameContentCorrupted(gameFileSystem.GetGameDirectory());
                }
            }

            if (!File.Exists(configFilePath))
            {
                return ChannelOptions.ConfigurationFileNotFound(configFilePath);
            }

            return GameConfiguration.Read(configFilePath, gameFileSystem);
        }
    }
}