// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Service.Game.Configuration;
using System.IO;

namespace Snap.Hutao.Service.Game.Launching;

internal sealed class LaunchExecutionSetChannelOptionsHandler : ILaunchExecutionDelegateHandler
{
    private const string ConfigFileName = "config.ini";

    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (!context.Options.TryGetGamePathAndFilePathByName(ConfigFileName, out string gamePath, out string? configPath))
        {
            context.Result.Kind = LaunchExecutionResultKind.NoActiveGamePath;
            context.Result.ErrorMessage = SH.ServiceGameLaunchExecutionSetChannelOptionsGamePathNotValid;
            return;
        }

        List<IniElement> elements = default!;
        try
        {
            using (FileStream readStream = File.OpenRead(configPath))
            {
                elements = [.. IniSerializer.Deserialize(readStream)];
            }
        }
        catch (FileNotFoundException)
        {
            context.Result.Kind = LaunchExecutionResultKind.GameConfigFileNotFound;
            context.Result.ErrorMessage = SH.FormatServiceGameSetMultiChannelConfigFileNotFound(configPath);
            return;
        }
        catch (DirectoryNotFoundException)
        {
            context.Result.Kind = LaunchExecutionResultKind.GameConfigDirectoryNotFound;
            context.Result.ErrorMessage = SH.FormatServiceGameSetMultiChannelConfigFileNotFound(configPath);
            return;
        }
        catch (UnauthorizedAccessException)
        {
            context.Result.Kind = LaunchExecutionResultKind.GameConfigUnauthorizedAccess;
            context.Result.ErrorMessage = SH.ServiceGameSetMultiChannelUnauthorizedAccess;
            return;
        }

        bool changed = false;

        foreach (IniElement element in elements)
        {
            if (element is IniParameter parameter)
            {
                if (parameter.Key is ChannelOptions.ChannelName)
                {
                    changed = parameter.Set(context.Scheme.Channel.ToString("D")) || changed;
                    continue;
                }

                if (parameter.Key is ChannelOptions.SubChannelName)
                {
                    changed = parameter.Set(context.Scheme.SubChannel.ToString("D")) || changed;
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

        await next().ConfigureAwait(false);
    }
}