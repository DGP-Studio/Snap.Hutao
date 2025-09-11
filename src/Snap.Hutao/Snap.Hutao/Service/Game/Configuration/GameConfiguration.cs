// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using System.Collections.Immutable;
using System.IO;

namespace Snap.Hutao.Service.Game.Configuration;

internal static class GameConfiguration
{
    public static ChannelOptions Read(string configFilePath, IGameFileSystem gameFileSystem)
    {
        ImmutableArray<IniElement> elements;
        try
        {
            elements = IniSerializer.DeserializeFromFile(configFilePath);
        }
        catch (IOException ex)
        {
            // The process cannot access the file '?' because it is being used by another process.
            if (HutaoNative.IsWin32(ex.HResult, WIN32_ERROR.ERROR_SHARING_VIOLATION))
            {
                return ChannelOptions.SharingViolation(configFilePath);
            }

            if (HutaoNative.IsWin32(ex.HResult, [WIN32_ERROR.ERROR_NOT_READY, WIN32_ERROR.ERROR_NO_SUCH_DEVICE]))
            {
                return ChannelOptions.DeviceNotFound(gameFileSystem.GetGameDirectory());
            }

            throw;
        }

        string? channel = default;
        string? subChannel = default;

        foreach (IniElement element in elements)
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

        return new(channel, subChannel, gameFileSystem.IsExecutableOversea());
    }
}