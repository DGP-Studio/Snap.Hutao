// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.IO.Ini;
using Snap.Hutao.Service.Game.Scheme;
using System.IO;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Service.Game;

internal static class GameFileSystemExtension
{
    public static bool TryGetGameVersion(this GameFileSystem gameFileSystem, [NotNullWhen(true)] out string? version)
    {
        version = default!;
        if (File.Exists(gameFileSystem.GameConfigFilePath))
        {
            foreach (ref readonly IniElement element in CollectionsMarshal.AsSpan(IniSerializer.DeserializeFromFile(gameFileSystem.GameConfigFilePath)))
            {
                if (element is IniParameter { Key: "game_version" } parameter)
                {
                    version = parameter.Value;
                    break;
                }
            }

            return true;
        }

        if (File.Exists(gameFileSystem.ScriptVersionFilePath))
        {
            version = File.ReadAllText(gameFileSystem.ScriptVersionFilePath);
            return true;
        }

        return false;
    }

    public static void GenerateConfigurationFile(this GameFileSystem gameFileSystem, string version, LaunchScheme launchScheme)
    {
        string gameBiz = launchScheme.IsOversea ? "hk4e_global" : "hk4e_cn";
        string content = $$$"""
            [General]
            channel={{{launchScheme.Channel:D}}}
            cps=mihoyo
            game_version={{{version}}}
            sub_channel={{{launchScheme.SubChannel:D}}}
            sdk_version=
            game_biz={{{gameBiz}}}
            uapc={"hk4e_cn":{"uapc":""},"hyp":{"uapc":""}}
            """;

        string? directory = Path.GetDirectoryName(gameFileSystem.GameConfigFilePath);
        ArgumentNullException.ThrowIfNull(directory);
        Directory.CreateDirectory(directory);
        File.WriteAllText(gameFileSystem.GameConfigFilePath, content);
    }

    public static void UpdateConfigurationFile(this GameFileSystem gameFileSystem, string version)
    {
        List<IniElement> ini = IniSerializer.DeserializeFromFile(gameFileSystem.GameConfigFilePath);
        IniParameter gameVersion = (IniParameter)ini.Single(e => e is IniParameter { Key: "game_version" });
        gameVersion.Set(version);
        IniSerializer.SerializeToFile(gameFileSystem.GameConfigFilePath, ini);
    }

    public static bool TryFixConfigurationFile(this GameFileSystem gameFileSystem, LaunchScheme launchScheme)
    {
        if (!File.Exists(gameFileSystem.ScriptVersionFilePath))
        {
            return false;
        }

        string version = File.ReadAllText(gameFileSystem.ScriptVersionFilePath);
        GenerateConfigurationFile(gameFileSystem, version, launchScheme);

        return true;
    }

    public static bool TryFixScriptVersion(this GameFileSystem gameFileSystem)
    {
        if (!File.Exists(gameFileSystem.GameConfigFilePath))
        {
            return false;
        }

        string? version = default;
        foreach (ref readonly IniElement element in CollectionsMarshal.AsSpan(IniSerializer.DeserializeFromFile(gameFileSystem.GameConfigFilePath)))
        {
            if (element is IniParameter { Key: "game_version" } parameter)
            {
                version = parameter.Value;
                break;
            }
        }

        string? directory = Path.GetDirectoryName(gameFileSystem.ScriptVersionFilePath);
        ArgumentNullException.ThrowIfNull(directory);

        Directory.CreateDirectory(directory);
        File.WriteAllText(gameFileSystem.ScriptVersionFilePath, version);
        return true;
    }
}