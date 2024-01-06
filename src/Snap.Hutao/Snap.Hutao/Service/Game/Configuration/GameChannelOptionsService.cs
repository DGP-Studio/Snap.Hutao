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
            return ChannelOptions.ConfigurationFileNotFound(gameFileSystem.GameConfigFilePath);
        }

        List<IniParameter> parameters = IniSerializer.DeserializeFromFile(gameFileSystem.GameConfigFilePath).OfType<IniParameter>().ToList();
        string? channel = parameters.FirstOrDefault(p => p.Key == ChannelOptions.ChannelName)?.Value;
        string? subChannel = parameters.FirstOrDefault(p => p.Key == ChannelOptions.SubChannelName)?.Value;

        return new(channel, subChannel, isOversea);
    }
}