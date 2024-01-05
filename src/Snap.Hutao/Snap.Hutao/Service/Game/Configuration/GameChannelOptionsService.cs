// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Service.Game.Scheme;
using System.IO;
using static Snap.Hutao.Service.Game.GameConstants;

namespace Snap.Hutao.Service.Game.Configuration;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IGameChannelOptionsService))]
internal sealed partial class GameChannelOptionsService : IGameChannelOptionsService
{
    private readonly LaunchOptions launchOptions;

    public ChannelOptions GetChannelOptions()
    {
        if (!launchOptions.TryGetGamePathAndFilePathByName(ConfigFileName, out string gamePath, out string? configPath))
        {
            throw ThrowHelper.InvalidOperation($"Invalid game path: {gamePath}");
        }

        bool isOversea = LaunchScheme.ExecutableIsOversea(Path.GetFileName(gamePath));

        if (!File.Exists(configPath))
        {
            return ChannelOptions.FileNotFound(isOversea, configPath);
        }

        using (FileStream stream = File.OpenRead(configPath))
        {
            List<IniParameter> parameters = IniSerializer.Deserialize(stream).OfType<IniParameter>().ToList();
            string? channel = parameters.FirstOrDefault(p => p.Key == ChannelOptions.ChannelName)?.Value;
            string? subChannel = parameters.FirstOrDefault(p => p.Key == ChannelOptions.SubChannelName)?.Value;

            return new(channel, subChannel, isOversea);
        }
    }
}