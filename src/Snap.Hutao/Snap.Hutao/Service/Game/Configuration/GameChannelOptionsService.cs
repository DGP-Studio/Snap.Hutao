// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Service.Game.Scheme;
using System.IO;

namespace Snap.Hutao.Service.Game.Configuration;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameChannelOptionsService))]
internal sealed partial class GameChannelOptionsService : IGameChannelOptionsService
{
    private readonly IGameConfigurationFileService gameConfigurationFileService;
    private readonly LaunchOptions launchOptions;

    public ChannelOptions GetChannelOptions()
    {
        if (!launchOptions.TryGetGameFileSystem(out GameFileSystem? gameFileSystem))
        {
            return ChannelOptions.GamePathNullOrEmpty();
        }

        bool isOversea = LaunchScheme.ExecutableIsOversea(gameFileSystem.GameFileName);

        if (!File.Exists(gameFileSystem.GameConfigFilePath))
        {
            // Try restore the configuration file if it does not exist
            // The configuration file may be deleted by a incompatible launcher
            gameConfigurationFileService.Restore(gameFileSystem.GameConfigFilePath);
        }

        if (!File.Exists(gameFileSystem.ScriptVersionFilePath))
        {
            // Try to fix ScriptVersion by reading game_version from the configuration file
            // Will check the configuration file first
            // If the configuration file and ScriptVersion file are both missing, the game content is corrupted
            if (!gameFileSystem.TryFixScriptVersion())
            {
                return ChannelOptions.GameContentCorrupted(gameFileSystem.GameDirectory);
            }
        }

        if (!File.Exists(gameFileSystem.GameConfigFilePath))
        {
            return ChannelOptions.ConfigurationFileNotFound(gameFileSystem.GameConfigFilePath);
        }

        List<IniParameter> parameters = IniSerializer.DeserializeFromFile(gameFileSystem.GameConfigFilePath).OfType<IniParameter>().ToList();
        string? channel = parameters.FirstOrDefault(p => p.Key is ChannelOptions.ChannelName)?.Value;
        string? subChannel = parameters.FirstOrDefault(p => p.Key is ChannelOptions.SubChannelName)?.Value;

        return new(channel, subChannel, isOversea);
    }
}