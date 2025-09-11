// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Win32;
using Snap.Hutao.Win32.Foundation;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.InteropServices;

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

    public static void Create(LaunchScheme launchScheme, string version, string configFilePath)
    {
        string gameBiz = launchScheme.IsOversea ? "hk4e_global" : "hk4e_cn";
        string content = $$$"""
            [general]
            uapc={"{{{gameBiz}}}":{"uapc":""},"hyp":{"uapc":""}}
            channel={{{launchScheme.Channel:D}}}
            sub_channel={{{launchScheme.SubChannel:D}}}
            cps=gw_pc
            game_version={{{version}}}
            """;

        string? directory = Path.GetDirectoryName(configFilePath);
        ArgumentNullException.ThrowIfNull(directory);
        Directory.CreateDirectory(directory);
        File.WriteAllText(configFilePath, content);
    }

    public static bool Patch(LaunchScheme launchScheme, string scriptVersionFilePath, string configFilePath)
    {
        if (!File.Exists(scriptVersionFilePath))
        {
            return false;
        }

        string version = File.ReadAllText(scriptVersionFilePath);
        Create(launchScheme, version, configFilePath);

        return true;
    }

    public static bool UpdateVersion(string configFilePath, string version)
    {
        bool updated = false;
        IniElement[]? ini = ImmutableCollectionsMarshal.AsArray(IniSerializer.DeserializeFromFile(configFilePath));

        if (ini is null)
        {
            return false;
        }

        foreach (ref IniElement element in ini.AsSpan())
        {
            if (element is not IniParameter { Key: "game_version" } parameter)
            {
                continue;
            }

            element = parameter.WithValue(version, out updated);
            break;
        }

        IniSerializer.SerializeToFile(configFilePath, ini);
        return updated;
    }
}