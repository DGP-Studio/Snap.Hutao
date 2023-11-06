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
    private readonly AppOptions appOptions;

    public ChannelOptions GetChannelOptions()
    {
        string gamePath = appOptions.GamePath;
        string configPath = Path.Combine(Path.GetDirectoryName(gamePath) ?? string.Empty, ConfigFileName);
        bool isOversea = string.Equals(Path.GetFileName(gamePath), GenshinImpactFileName, StringComparison.OrdinalIgnoreCase);

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
        string gamePath = appOptions.GamePath;
        string? directory = Path.GetDirectoryName(gamePath);
        ArgumentException.ThrowIfNullOrEmpty(directory);
        string configPath = Path.Combine(directory, ConfigFileName);

        List<IniElement> elements = default!;
        try
        {
            using (FileStream readStream = File.OpenRead(configPath))
            {
                elements = IniSerializer.Deserialize(readStream).ToList();
            }
        }
        catch (FileNotFoundException ex)
        {
            ThrowHelper.GameFileOperation(SH.ServiceGameSetMultiChannelConfigFileNotFound.Format(configPath), ex);
        }
        catch (DirectoryNotFoundException ex)
        {
            ThrowHelper.GameFileOperation(SH.ServiceGameSetMultiChannelConfigFileNotFound.Format(configPath), ex);
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
                if (parameter.Key == "channel")
                {
                    changed = parameter.Set(scheme.Channel.ToString("D")) || changed;
                }

                if (parameter.Key == "sub_channel")
                {
                    changed = parameter.Set(scheme.SubChannel.ToString("D")) || changed;
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