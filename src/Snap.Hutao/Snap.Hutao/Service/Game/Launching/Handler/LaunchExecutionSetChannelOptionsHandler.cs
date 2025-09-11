// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Service.Game.Configuration;
using Snap.Hutao.Service.Game.FileSystem;
using System.IO;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionSetChannelOptionsHandler : ILaunchExecutionDelegateHandler
{
    public ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        return next();
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (!context.TryGetGameFileSystem(out IGameFileSystemView? gameFileSystem))
        {
            // context.Result is set in TryGetGameFileSystem
            return;
        }

        string configPath = gameFileSystem.GetGameConfigurationFilePath();
        context.Logger.LogInformation("Game config file path: {ConfigPath}", configPath);

        IniElement[]? elements;
        try
        {
            elements = ImmutableCollectionsMarshal.AsArray(IniSerializer.DeserializeFromFile(configPath));
            ArgumentNullException.ThrowIfNull(elements);
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

        SetChannelOptions(context, elements);

        if (context.ChannelOptionsChanged)
        {
            IniSerializer.SerializeToFile(configPath, elements);
        }

        // Backup config file, recover when an incompatible launcher deleted it.
        // Config file has already been overwritten as target scheme at the moment.
        //
        // We should backup config file every time we launch the game due to the possibility of the game version outdated.
        context.ServiceProvider.GetRequiredService<IGameConfigurationFileService>()
            .Backup(gameFileSystem.GetGameConfigurationFilePath(), gameFileSystem.IsExecutableOversea());

        await next().ConfigureAwait(false);
    }

    private static void SetChannelOptions(LaunchExecutionContext context, IniElement[] elements)
    {
        foreach (ref IniElement element in elements.AsSpan())
        {
            if (element is not IniParameter parameter)
            {
                continue;
            }

            switch (parameter.Key)
            {
                case ChannelOptions.ChannelName:
                    {
                        element = parameter.WithValue(context.TargetScheme.Channel.ToString("D"), out bool changed);
                        context.ChannelOptionsChanged = changed || context.ChannelOptionsChanged;
                        continue;
                    }

                case ChannelOptions.SubChannelName:
                    {
                        element = parameter.WithValue(context.TargetScheme.SubChannel.ToString("D"), out bool changed);
                        context.ChannelOptionsChanged = changed || context.ChannelOptionsChanged;
                        continue;
                    }
            }
        }
    }
}