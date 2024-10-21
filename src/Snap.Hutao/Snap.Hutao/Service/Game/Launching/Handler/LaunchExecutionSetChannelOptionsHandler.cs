// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Service.Game.Configuration;
using System.IO;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionSetChannelOptionsHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (!context.TryGetGameFileSystem(out GameFileSystem? gameFileSystem))
        {
            // context.Result is set in TryGetGameFileSystem
            return;
        }

        string configPath = gameFileSystem.GameConfigFilePath;
        context.Logger.LogInformation("Game config file path: {ConfigPath}", configPath);

        List<IniElement> elements;
        try
        {
            elements = [.. IniSerializer.DeserializeFromFile(configPath)];
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
            context.Result.Kind = LaunchExecutionResultKind.GameConfigInsufficientPermissions;
            context.Result.ErrorMessage = SH.ServiceGameSetMultiChannelUnauthorizedAccess;
            return;
        }

        foreach (IniElement element in elements)
        {
            if (element is IniParameter parameter)
            {
                if (parameter.Key is ChannelOptions.ChannelName)
                {
                    context.ChannelOptionsChanged = parameter.Set(context.TargetScheme.Channel.ToString("D")) || context.ChannelOptionsChanged;
                    continue;
                }

                if (parameter.Key is ChannelOptions.SubChannelName)
                {
                    context.ChannelOptionsChanged = parameter.Set(context.TargetScheme.SubChannel.ToString("D")) || context.ChannelOptionsChanged;
                    continue;
                }
            }
        }

        if (context.ChannelOptionsChanged)
        {
            IniSerializer.SerializeToFile(configPath, elements);
        }

        await next().ConfigureAwait(false);
    }
}