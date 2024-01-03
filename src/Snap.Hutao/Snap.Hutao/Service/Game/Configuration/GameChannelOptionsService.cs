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

    public bool SetChannelOptions(LaunchScheme scheme)
    {
        if (!launchOptions.TryGetGamePathAndFilePathByName(ConfigFileName, out string gamePath, out string? configPath))
        {
            return false;
        }

        List<IniElement> elements = default!;
        try
        {
            using (FileStream readStream = File.OpenRead(configPath))
            {
                elements = [.. IniSerializer.Deserialize(readStream)];
            }
        }
        catch (FileNotFoundException ex)
        {
            ThrowHelper.GameFileOperation(SH.FormatServiceGameSetMultiChannelConfigFileNotFound(configPath), ex);
        }
        catch (DirectoryNotFoundException ex)
        {
            ThrowHelper.GameFileOperation(SH.FormatServiceGameSetMultiChannelConfigFileNotFound(configPath), ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            ThrowHelper.GameFileOperation(SH.ServiceGameSetMultiChannelUnauthorizedAccess, ex);
        }

        bool changed = false;

        foreach (IniElement element in elements)
        {
            if (element is IniParameter parameter)
            {
                if (parameter.Key is ChannelOptions.ChannelName)
                {
                    changed = parameter.Set(scheme.Channel.ToString("D")) || changed;
                    continue;
                }

                if (parameter.Key is ChannelOptions.SubChannelName)
                {
                    changed = parameter.Set(scheme.SubChannel.ToString("D")) || changed;
                    continue;
                }
            }
        }

        if (changed)
        {
            using (FileStream writeStream = File.Create(configPath))
            {
                IniSerializer.Serialize(writeStream, elements);
            }
        }

        return changed;
    }
}