// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Service.Game.Scheme;
using System.IO;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Service.Game.Configuration;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameChannelOptionsService))]
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

            string? channel = default;
            string? subChannel = default;
            foreach (ref readonly IniElement element in IniSerializer.DeserializeFromFile(gameFileSystem.GameConfigFilePath).AsSpan())
            {
                if (element is not IniParameter parameter)
                {
                    continue;
                }

                switch (parameter.Key)
                {
                    case ChannelOptions.ChannelName:
                        channel = parameter.Value;
                        break;
                    case ChannelOptions.SubChannelName:
                        subChannel = parameter.Value;
                        break;
                }

                if (channel is not null && subChannel is not null)
                {
                    break;
                }
            }

            return new(channel, subChannel, isOversea);
        }
    }
}