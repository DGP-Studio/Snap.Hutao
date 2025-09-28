// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Service.Game.Configuration;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Launching.Context;
using System.IO;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionChannelOptionsHandler : AbstractLaunchExecutionHandler
{
    public override ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        string configPath = context.FileSystem.GetGameConfigurationFilePath();

        IniElement[]? elements;
        try
        {
            elements = ImmutableCollectionsMarshal.AsArray(IniSerializer.DeserializeFromFile(configPath));
            ArgumentNullException.ThrowIfNull(elements);
        }
        catch (FileNotFoundException fnfEx)
        {
            return ValueTask.FromException(HutaoException.NotSupported(SH.FormatServiceGameSetMultiChannelConfigFileNotFound(configPath), fnfEx));
        }
        catch (DirectoryNotFoundException dnfEx)
        {
            return ValueTask.FromException(HutaoException.NotSupported(SH.FormatServiceGameSetMultiChannelConfigFileNotFound(configPath), dnfEx));
        }
        catch (UnauthorizedAccessException uaEx)
        {
            return ValueTask.FromException(HutaoException.NotSupported(SH.ServiceGameSetMultiChannelUnauthorizedAccess, uaEx));
        }

        UpdateChannelOptionsFromTargetLaunchScheme(elements, context);

        if (context.TryGetOption(LaunchExecutionOptionsKey.ChannelOptionsChanged, out bool changed) && changed)
        {
            IniSerializer.SerializeToFile(configPath, elements);
        }

        // Backup config file, recover when an incompatible launcher deleted it.
        // Config file has already been overwritten as target scheme at the moment.
        // We should backup config file every time we launch the game due to the possibility of the game version outdated.
        context.ServiceProvider.GetRequiredService<IGameConfigurationFileService>()
            .Backup(context.FileSystem.GetGameConfigurationFilePath(), context.FileSystem.IsExecutableOversea());

        return ValueTask.CompletedTask;
    }

    private static void UpdateChannelOptionsFromTargetLaunchScheme(IniElement[] elements, BeforeLaunchExecutionContext context)
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
                        context.TryGetOption(LaunchExecutionOptionsKey.ChannelOptionsChanged, out bool previous);
                        context.SetOption(LaunchExecutionOptionsKey.ChannelOptionsChanged, changed || previous);
                        continue;
                    }

                case ChannelOptions.SubChannelName:
                    {
                        element = parameter.WithValue(context.TargetScheme.SubChannel.ToString("D"), out bool changed);
                        context.TryGetOption(LaunchExecutionOptionsKey.ChannelOptionsChanged, out bool previous);
                        context.SetOption(LaunchExecutionOptionsKey.ChannelOptionsChanged, changed || previous);
                        continue;
                    }
            }
        }
    }
}