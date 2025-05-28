// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
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
                // The configuration file may be deleted by an incompatible launcher
                gameConfigurationFileService.Restore(configFilePath, gameFileSystem.IsOversea());
            }

            if (!File.Exists(gameFileSystem.GetScriptVersionFilePath()))
            {
                // Try to fix ScriptVersion by reading game_version from the configuration file
                // Will check the configuration file first
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

            string? channel = default;
            string? subChannel = default;

            ReadOnlySpan<IniElement> elements;
            try
            {
                elements = IniSerializer.DeserializeFromFile(configFilePath).AsSpan();
            }
            catch (IOException ex)
            {
                if (HutaoNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_NOT_READY))
                {
                    return ChannelOptions.GameContentCorrupted(gameFileSystem.GetGameDirectory());
                }

                throw;
            }

            foreach (ref readonly IniElement element in elements)
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

            return new(channel, subChannel, gameFileSystem.IsOversea());
        }
    }
}